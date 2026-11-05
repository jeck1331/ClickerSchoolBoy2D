using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class WhackMoleManager : MiniGameBase
{
    [Header("Session")]
    [SerializeField, Min(1f)] private float sessionDuration = 60f;
    [SerializeField] private bool startAutomatically = true;
    [Header("Scene holes and mole prefabs")]
    [SerializeField] private WhackMoleHole[] holes;
    [SerializeField] private WhackMole[] molePrefabs;
    [Header("Rewards (same assets as GameManager)")]
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private string exitSceneName = "MainScene";

    private WhackMole[,] pool;
    private WhackMole[] activeMoles;
    private float[] spawnDelays;
    private float sessionLength;
    private int displayedSecond = -1;
    private bool applicationPaused;

    public int Score { get; private set; }
    public long Reward { get; private set; }
    public float RemainingTime { get; private set; }
    public bool HasStarted { get; private set; }
    public bool HasEnded { get; private set; }
    public bool IsRunning => isActiveAndEnabled && HasStarted && !HasEnded && !applicationPaused &&
                             !GameplayPauseService.IsGameplayInputPaused && Time.timeScale > 0f;

    private void Start()
    {
        if (startAutomatically) StartSession();
    }

    public void StartSession()
    {
        if (!isActiveAndEnabled || (HasStarted && !HasEnded)) return;
        if (pool == null && !CreatePool()) return;
        ResetMoles();
        Score = 0;
        Reward = 0;
        sessionLength = Mathf.Max(1f, sessionDuration);
        RemainingTime = sessionLength;
        for (int i = 0; i < spawnDelays.Length; i++)
            spawnDelays[i] = Random.Range(1f, 3f);
        displayedSecond = -1;
        HasStarted = true;
        HasEnded = false;
        if (resultScreen != null) resultScreen.SetActive(false);
        UpdateScoreUI();
        UpdateTimeUI();
    }

    private bool CreatePool()
    {
        if (holes == null || holes.Length == 0 || molePrefabs == null || molePrefabs.Length == 0 ||
            powerValue == null || incomeValue == null || scoreValue == null ||
            Array.Exists(molePrefabs, mole => mole == null || !mole.IsConfigured))
        {
            Debug.LogError("WhackMole: assign non-empty hole and mole lists, mole prefabs with both sprites and all three reward assets.", this);
            return false;
        }
        for (int i = 0; i < holes.Length; i++)
        {
            if (holes[i] != null && holes[i].IsConfigured) continue;
            Debug.LogError($"WhackMole: Holes[{i}] ({(holes[i] != null ? holes[i].name : "None")}) needs a SortingGroup and assigned child Hidden Point, Visible Point and Sprite Mask.",
                holes[i] != null ? holes[i] : this);
            return false;
        }
        for (int i = 0; i < holes.Length; i++)
        for (int j = 0; j < i; j++)
            if (holes[i] == holes[j])
            {
                Debug.LogError("WhackMole: each hole must appear only once in the list.", this);
                return false;
            }

        pool = new WhackMole[holes.Length, molePrefabs.Length];
        activeMoles = new WhackMole[holes.Length];
        spawnDelays = new float[holes.Length];
        // Prewarm every variant per hole once; sessions only reuse these instances.
        for (int i = 0; i < holes.Length; i++)
        for (int j = 0; j < molePrefabs.Length; j++)
        {
            WhackMole mole = Instantiate(molePrefabs[j], holes[i].transform);
            mole.Initialize(this, holes[i]);
            pool[i, j] = mole;
        }
        return true;
    }

    private void Update()
    {
        if (!IsRunning) return;
        float deltaTime = Time.deltaTime;
        RemainingTime = Mathf.Max(0f, RemainingTime - deltaTime);
        if (RemainingTime <= 0f) { EndSession(); return; }

        float fraction = RemainingTime / sessionLength;
        float frequency = fraction <= 0.1f ? 1.4f : fraction <= 0.5f ? 1.25f : 1f;
        float maxLifetime = fraction <= 0.1f ? 1.3f : fraction <= 0.5f ? 1.6f : 2f;
        for (int i = 0; i < activeMoles.Length; i++)
        {
            WhackMole mole = activeMoles[i];
            if (mole != null)
            {
                mole.Tick(deltaTime);
                if (mole.IsHidden)
                {
                    activeMoles[i] = null;
                    spawnDelays[i] = Random.Range(1f, 3f);
                }
                continue;
            }

            // Every free hole has its own clock; appearances can freely overlap.
            spawnDelays[i] -= deltaTime * frequency;
            if (spawnDelays[i] > 0f) continue;
            mole = pool[i, Random.Range(0, pool.GetLength(1))];
            activeMoles[i] = mole;
            mole.Show(Random.Range(0.4f, maxLifetime));
        }
        UpdateTimeUI();
    }

    public void RegisterHit()
    {
        if (!IsRunning) return;
        if (Score < int.MaxValue) Score++;
        UpdateScoreUI();
    }

    public void EndSession()
    {
        if (!HasStarted || HasEnded) return;
        HasEnded = true;
        RemainingTime = 0f;
        ResetMoles();
        decimal passive = incomeValue.Value <= 0 ? 1.25m : (decimal)incomeValue.Value * 0.3m;
        decimal factor = passive + (decimal)Mathf.Max(0, powerValue.Value) * 1.05m;
        Reward = (long)Math.Min(long.MaxValue, Math.Ceiling(Score * factor));
        if (Reward > 0)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddCoinsAndSave(Reward);
            else scoreValue.Value = new SaveGameSystem().AddCoins(Reward);
        }
        UpdateTimeUI();
        if (resultText != null) resultText.text = $"Очки: {Score}\nНаграда: {Reward}";
        if (resultScreen != null) resultScreen.SetActive(true);
    }

    public void ExitGame()
    {
        // Leaving an unfinished session does not pay a completion reward.
        SceneManager.LoadScene(exitSceneName);
    }

    private void ResetMoles()
    {
        if (activeMoles == null) return;
        for (int i = 0; i < activeMoles.Length; i++)
        {
            if (activeMoles[i] != null) activeMoles[i].ResetMole();
            activeMoles[i] = null;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = Score.ToString();
    }

    private void UpdateTimeUI()
    {
        int second = Mathf.CeilToInt(RemainingTime);
        if (second == displayedSecond) return;
        displayedSecond = second;
        if (timeText != null) timeText.text = second.ToString();
    }

    private void OnApplicationPause(bool paused) => applicationPaused = paused;

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetMoles();
        HasStarted = false;
    }
}
