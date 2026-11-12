using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PopItManager : MiniGameBase
{
    [Header("Session")]
    [SerializeField, Min(1f)] private float sessionDuration = 60f;
    [SerializeField] private bool startAutomatically = true;
    [Header("Audio")]
    [SerializeField] private AudioClip bubblePopClip;
    [Header("Board (one two-state bubble in each row)")]
    [SerializeField] private SpriteRenderer popitZone;
    [SerializeField] private Camera worldCamera;
    [SerializeField, Min(1)] private int bubblesPerLine = 6;
    [SerializeField, Range(0f, 0.49f)] private float horizontalPadding = 0.12f;
    [Header("Recovery")]
    [SerializeField, Min(0.1f)] private float minRecoveryDelay = 3f;
    [SerializeField, Min(0.1f)] private float maxRecoveryDelay = 7f;
    [SerializeField, Range(0f, 1f)] private float accelerationThreshold = 0.2f;
    [SerializeField, Min(0f)] private float recoverySpeedBonus = 0.3f;
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
    [Header("Score popup")]
    [SerializeField, Min(0.01f)] private float popupDuration = 0.7f;
    [SerializeField, Min(0f)] private float popupRise = 100f;

    private readonly List<PopItBubble> bubbles = new();
    private int unpressedCount;
    private int displayedSecond = -1;
    private bool applicationPaused;

    public int Score { get; private set; }
    public long Reward { get; private set; }
    public float RemainingTime { get; private set; }
    public bool HasStarted { get; private set; }
    public bool HasEnded { get; private set; }
    public Camera WorldCamera => worldCamera;
    public float PopupDuration => Mathf.Max(0.01f, popupDuration);
    public float PopupRise => popupRise;
    public bool IsRunning => isActiveAndEnabled && HasStarted && !HasEnded && !applicationPaused &&
                             !GameplayPauseService.IsGameplayInputPaused && Time.timeScale > 0f;

    private void Start()
    {
        if (startAutomatically) StartSession();
    }

    public void StartSession()
    {
        if (!isActiveAndEnabled || (HasStarted && !HasEnded)) return;
        if (bubbles.Count == 0 && !CreateBoard()) return;
        foreach (PopItBubble bubble in bubbles) bubble.ResetBubble();
        unpressedCount = bubbles.Count;
        Score = 0;
        Reward = 0;
        RemainingTime = Mathf.Max(1f, sessionDuration);
        displayedSecond = -1;
        HasStarted = true;
        HasEnded = false;
        if (resultScreen != null) resultScreen.SetActive(false);
        UpdateScoreUI();
        UpdateTimeUI();
    }

    private bool CreateBoard()
    {
        if (popitZone == null || worldCamera == null || scoreValue == null || powerValue == null || incomeValue == null ||
            popitZone.transform.childCount == 0)
        {
            Debug.LogError("PopIt: assign the board, camera and all three reward assets.", this);
            return false;
        }
        foreach (Transform line in popitZone.transform)
        {
            if (line.childCount != 1 || line.GetChild(0).childCount != 2 ||
                line.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>() == null ||
                line.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>() == null)
            {
                Debug.LogError("PopIt: each row needs one bubble with normal and pressed SpriteRenderer children, in that order.", line);
                return false;
            }
        }

        int count = Mathf.Max(1, bubblesPerLine);
        Bounds board = popitZone.bounds;
        float inset = board.size.x * Mathf.Clamp(horizontalPadding, 0f, 0.49f);
        foreach (Transform line in popitZone.transform)
        {
            Transform template = line.GetChild(0);
            // Clone before initializing so each instance gets its own state and popup.
            Transform[] row = new Transform[count];
            row[0] = template;
            for (int i = 1; i < count; i++) row[i] = Instantiate(template, line);
            for (int i = 0; i < count; i++)
            {
                SpriteRenderer normal = row[i].GetChild(0).GetComponent<SpriteRenderer>();
                float x = count == 1 ? board.center.x : Mathf.Lerp(board.min.x + inset, board.max.x - inset, (float)i / (count - 1));
                // Art pivots are offset from the bubble root; align using the visible sprite's center.
                row[i].position += Vector3.right * (x - normal.bounds.center.x);
                PopItBubble bubble = row[i].GetComponent<PopItBubble>();
                if (bubble == null) bubble = row[i].gameObject.AddComponent<PopItBubble>();
                bubble.Initialize(this, scoreText);
                bubbles.Add(bubble);
            }
        }
        return true;
    }

    private void Update()
    {
        if (!IsRunning) return;
        float deltaTime = Time.deltaTime;
        RemainingTime = Mathf.Max(0f, RemainingTime - deltaTime);
        if (RemainingTime <= 0f) { EndSession(); return; }

        float speed = unpressedCount <= bubbles.Count * accelerationThreshold ? 1f + recoverySpeedBonus : 1f;
        foreach (PopItBubble bubble in bubbles)
            if (bubble.Tick(deltaTime, speed)) unpressedCount++;
        UpdateTimeUI();
    }

    public float GetRecoveryDelay()
    {
        float min = Mathf.Max(0.1f, minRecoveryDelay);
        return Random.Range(min, Mathf.Max(min, maxRecoveryDelay));
    }

    public void RegisterPress()
    {
        if (!IsRunning) return;
        AudioManager.Instance?.PlaySfx(bubblePopClip);
        unpressedCount--;
        if (Score < int.MaxValue) Score++;
        UpdateScoreUI();
    }

    public void EndSession()
    {
        if (!HasStarted || HasEnded) return;
        HasEnded = true;
        RemainingTime = 0f;
        foreach (PopItBubble bubble in bubbles) bubble.HidePopup();
        decimal passive = incomeValue.Value <= 0 ? 1.5m : (decimal)incomeValue.Value * 1.5m;
        decimal factor = passive + (decimal)Mathf.Max(0, powerValue.Value) * 1.2m;
        Reward = (long)Math.Min(long.MaxValue, Math.Ceiling(Score * factor));
        if (Reward > 0)
        {
            if (GameManager.Instance != null) GameManager.Instance.AddCoinsAndSave(Reward);
            else scoreValue.Value = new SaveGameSystem().AddCoins(Reward);
        }
        UpdateTimeUI();
        if (resultText != null) resultText.text = $"Очки: {Score}\nМонеты: {Reward}";
        if (resultScreen != null) resultScreen.SetActive(true);
    }

    public void ExitGame() => SceneManager.LoadScene(exitSceneName);

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
        foreach (PopItBubble bubble in bubbles) bubble.HidePopup();
        HasStarted = false;
    }
}
