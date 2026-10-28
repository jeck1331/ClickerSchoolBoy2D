using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FruitSchoolController : MiniGameBase
{
    [Header("Session")]
    [SerializeField, Min(1f)] private float sessionDuration = 60f;
    [SerializeField, Min(1)] private int startingLives = 3;
    [SerializeField] private bool startAutomatically = true;
    [Header("Play area")]
    [SerializeField] private RectTransform gameZoneUI;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float gameplayZ;
    [Header("Spawning")]
    [SerializeField] private FruitSchoolItem[] fruitPrefabs;
    [SerializeField] private FruitSchoolItem largeBombPrefab;
    [SerializeField] private FruitSchoolItem smallBombPrefab;
    [SerializeField, Range(0f, 1f)] private float bombChance = 0.15f;
    [SerializeField] private Vector2 waveInterval = new Vector2(0.55f, 0.9f);
    [SerializeField] private Vector2Int fruitsPerWave = new Vector2Int(2, 4);
    [SerializeField, Min(0.1f)] private float gravity = 12f;
    [SerializeField, Range(0.2f, 0.95f)] private float minimumHeight = 0.55f;
    [SerializeField, Range(0.2f, 0.95f)] private float maximumHeight = 0.85f;
    [Header("Effects")]
    [Tooltip("Prefab or scene template. A separate copy plays for each tap; the template itself is not played.")]
    [SerializeField] private ParticleSystem cutParticleEffect;
    [Header("Rewards (same assets as GameManager)")]
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    [SerializeField, Min(0)] private int clickMultiplier = 3;
    [SerializeField, Min(0)] private int incomeMultiplier = 10;
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private UnityEvent onSessionStarted = new UnityEvent();
    [SerializeField] private UnityEvent onSessionEnded = new UnityEvent();

    private readonly List<FruitSchoolItem> items = new List<FruitSchoolItem>();
    private readonly List<ParticleSystem> effects = new List<ParticleSystem>();
    private readonly Vector3[] corners = new Vector3[4];
    private Camera uiCamera;
    private float nextWave;
    private bool rewardPaid;
    private int spawnOrder;

    public long Score { get; private set; }
    public long Reward { get; private set; }
    public int Lives { get; private set; }
    public float RemainingTime { get; private set; }
    public bool HasStarted { get; private set; }
    public bool HasEnded { get; private set; }
    public bool IsRunning => isActiveAndEnabled && HasStarted && !HasEnded &&
                             !GameplayPauseService.IsGameplayInputPaused && Time.timeScale > 0f;

    private void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
        if (gameZoneUI != null)
        {
            Canvas canvas = gameZoneUI.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                uiCamera = canvas.rootCanvas.worldCamera;
        }
        if (resultScreen != null) resultScreen.SetActive(false);
    }

    private void Start()
    {
        if (startAutomatically) StartSession();
    }

    public void StartSession()
    {
        if (!isActiveAndEnabled || (HasStarted && !HasEnded)) return;
        if (HasEnded) PayReward();
        if (Physics2D.gravity.y >= 0f || !Mathf.Approximately(Physics2D.gravity.x, 0f))
        {
            Debug.LogError("FruitSchool: Physics 2D gravity must point down (X = 0, Y < 0).", this);
            return;
        }
        if (gameZoneUI == null || worldCamera == null || powerValue == null || incomeValue == null ||
            fruitPrefabs == null || fruitPrefabs.Length == 0 ||
            System.Array.Exists(fruitPrefabs, prefab => prefab == null || !prefab.IsConfigured ||
                                prefab.Kind != FruitSchoolItem.ItemKind.Fruit) ||
            (largeBombPrefab != null && (!largeBombPrefab.IsConfigured || largeBombPrefab.Kind != FruitSchoolItem.ItemKind.LargeBomb)) ||
            (smallBombPrefab != null && (!smallBombPrefab.IsConfigured || smallBombPrefab.Kind != FruitSchoolItem.ItemKind.SmallBomb)))
        {
            Debug.LogError("FruitSchool: check the play area, camera, reward assets, prefab kinds and child form references.", this);
            return;
        }
        ClearObjects();
        Score = 0;
        Reward = 0;
        Lives = Mathf.Max(1, startingLives);
        RemainingTime = Mathf.Max(1f, sessionDuration);
        nextWave = 0f;
        spawnOrder = 0;
        rewardPaid = false;
        HasStarted = true;
        HasEnded = false;
        if (resultScreen != null) resultScreen.SetActive(false);
        UpdateUI();
        onSessionStarted.Invoke();
    }

    private void Update()
    {
        if (GameplayPauseService.IsGameplayInputPaused || Time.timeScale <= 0f) return;
        TickEffects(Time.deltaTime);
        if (!IsRunning || !TryGetBounds(out Bounds bounds)) return;
        float dt = Time.deltaTime;
        RemainingTime = Mathf.Max(0f, RemainingTime - dt);
        if (RemainingTime <= 0f) { EndSession(); return; }

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (!items[i].Tick(dt, Mathf.Max(0.1f, gravity), bounds.min.y - 2f)) continue;
            Destroy(items[i].gameObject);
            items.RemoveAt(i);
        }
        nextWave -= dt;
        if (nextWave <= 0f)
        {
            int count = Random.Range(Mathf.Max(1, fruitsPerWave.x), Mathf.Max(1, fruitsPerWave.y) + 1);
            for (int i = 0; i < count; i++) Spawn(bounds);
            nextWave = Random.Range(Mathf.Max(0.1f, waveInterval.x), Mathf.Max(0.1f, waveInterval.y));
        }
        ReadInput();
        UpdateUI();
    }

    private void Spawn(Bounds bounds)
    {
        FruitSchoolItem prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
        if (Random.value < bombChance)
        {
            FruitSchoolItem bomb = Random.value < 0.5f ? largeBombPrefab : smallBombPrefab;
            if (bomb != null) prefab = bomb;
        }
        float x = Random.Range(bounds.min.x + bounds.size.x * 0.15f, bounds.max.x - bounds.size.x * 0.15f);
        float y = bounds.min.y - 1f;
        float peak = bounds.min.y + bounds.size.y * Random.Range(minimumHeight, maximumHeight);
        float speed = Mathf.Sqrt(2f * Mathf.Max(0.1f, gravity) * (peak - y));
        float targetX = Random.Range(bounds.min.x + bounds.size.x * 0.2f, bounds.max.x - bounds.size.x * 0.2f);
        FruitSchoolItem item = Instantiate(prefab, new Vector3(x, y, gameplayZ), Quaternion.identity, transform);
        item.gameObject.SetActive(true);
        item.Launch(new Vector2((targetX - x) / (2f * speed / Mathf.Max(0.1f, gravity)), speed),
            Random.Range(-150f, 150f), ++spawnOrder, Mathf.Max(0.1f, gravity));
        items.Add(item);
    }

    private void ReadInput()
    {
        bool touched = false;
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed) touched = true;
                if (touch.press.wasPressedThisFrame) TryPop(touch.position.ReadValue());
            }
        }
        if (!touched && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryPop(Mouse.current.position.ReadValue());
    }

    private void TryPop(Vector2 screenPoint)
    {
        if (!IsRunning || !RectTransformUtility.RectangleContainsScreenPoint(gameZoneUI, screenPoint, uiCamera)) return;
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, gameplayZ));
        Ray ray = worldCamera.ScreenPointToRay(screenPoint);
        if (!plane.Raycast(ray, out float distance)) return;
        Vector3 point = ray.GetPoint(distance);
        // Newer objects are drawn on top; one tap consumes only one object.
        for (int i = items.Count - 1; i >= 0; i--)
        {
            FruitSchoolItem item = items[i];
            if (!item.Contains(point)) continue;
            item.Pop();
            PlayEffect(item.transform.position);
            if (item.Kind == FruitSchoolItem.ItemKind.Fruit)
                Score += System.Math.Min((long)Mathf.Max(0, item.Points), long.MaxValue - Score);
            else
            {
                bool small = item.Kind == FruitSchoolItem.ItemKind.SmallBomb;
                Lives = Mathf.Max(0, Lives - 1);
                RemainingTime = Mathf.Max(0f, RemainingTime - (small ? 10f : 5f));
                if (small) Score = System.Math.Max(0L, Score - 50L);
            }
            if (Lives == 0 || RemainingTime <= 0f) EndSession();
            return;
        }
    }

    private void PlayEffect(Vector3 position)
    {
        if (cutParticleEffect == null) return;
        ParticleSystem effect = Instantiate(cutParticleEffect, position, cutParticleEffect.transform.rotation, transform);
        effect.gameObject.SetActive(true);
        effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        foreach (ParticleSystem child in effect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = child.main;
            main.loop = false;
            main.stopAction = ParticleSystemStopAction.None;
        }
        effect.Simulate(0f, true, true);
        foreach (var renderer in effect.GetComponentsInChildren<ParticleSystemRenderer>(true))
            renderer.sortingOrder = 32767;
        effects.Add(effect);
    }

    private void TickEffects(float deltaTime)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            // Manual simulation gives each burst its own lifetime and respects gameplay pause.
            effects[i].Simulate(deltaTime, true, false);
            if (effects[i].IsAlive(true)) continue;
            Destroy(effects[i].gameObject);
            effects.RemoveAt(i);
        }
    }

    public void EndSession()
    {
        if (!HasStarted || HasEnded) return;
        HasEnded = true;
        // Decimal avoids overflow before clamping to the balance's long representation.
        decimal factor = (decimal)Mathf.Max(0, powerValue.Value) * Mathf.Max(0, clickMultiplier) +
                         (incomeValue.Value <= 0 ? 5m : (decimal)incomeValue.Value * Mathf.Max(0, incomeMultiplier));
        Reward = (long)System.Math.Min(long.MaxValue, Score * factor);
        foreach (var item in items) if (item != null) Destroy(item.gameObject);
        items.Clear();
        UpdateUI();
        if (resultText != null) resultText.text = $"Score: {Score}\nReward: {Reward}";
        if (resultScreen != null) resultScreen.SetActive(true);
        PayReward();
        onSessionEnded.Invoke();
    }

    private void PayReward()
    {
        if (rewardPaid || Reward <= 0) return;
        if (GameManager.Instance != null) GameManager.Instance.AddCoinsAndSave(Reward);
        else
        {
            long balance = new SaveGameSystem().AddCoins(Reward);
            if (scoreValue != null) scoreValue.Value = balance;
        }
        rewardPaid = true;
    }

    private bool TryGetBounds(out Bounds bounds)
    {
        bounds = default;
        gameZoneUI.GetWorldCorners(corners);
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, gameplayZ));
        for (int i = 0; i < corners.Length; i++)
        {
            Ray ray = worldCamera.ScreenPointToRay(RectTransformUtility.WorldToScreenPoint(uiCamera, corners[i]));
            if (!plane.Raycast(ray, out float distance)) return false;
            if (i == 0) bounds = new Bounds(ray.GetPoint(distance), Vector3.zero);
            else bounds.Encapsulate(ray.GetPoint(distance));
        }
        return bounds.size.x > 0f && bounds.size.y > 0f;
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = Score.ToString();
        if (timeText != null) timeText.text = $"Time: {Mathf.CeilToInt(RemainingTime)}";
        if (livesText != null) livesText.text = $"Lives: {Lives}";
    }

    protected override void HandlePause(bool paused)
    {
        foreach (var item in items) item.SetPaused(paused || !IsRunning);
    }

    private void FixedUpdate()
    {
        foreach (var item in items) item.SetPaused(!IsRunning);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ClearObjects();
        HasStarted = false;
    }

    private void ClearObjects()
    {
        foreach (var item in items) if (item != null) Destroy(item.gameObject);
        foreach (var effect in effects) if (effect != null) Destroy(effect.gameObject);
        items.Clear();
        effects.Clear();
    }
}
