using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PinguinController : MonoBehaviour
{
    [SerializeField] private FlappyGameZone gameZone;
    [SerializeField, Min(0.1f)] private float flapSpeed = 4f;
    [SerializeField, Min(0.1f)] private float maxFallSpeed = 7f;
    [SerializeField] private LayerMask collisionLayers = ~0;
    [Header("Score and rewards")]
    [Tooltip("Assign the same value assets used by the main game's GameManager.")]
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    [SerializeField, Min(0)] private int clickMultiplier = 3;
    [SerializeField, Min(0)] private int incomeMultiplier = 10;
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text earnedText;
    [SerializeField] private GameObject refreshScreen;
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer playerRenderer;
    [Tooltip("If empty, uses the sprite already assigned to the SpriteRenderer.")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite divingSprite;
    [Tooltip("Downward speed required to show the diving sprite. Zero switches as soon as the player falls.")]
    [SerializeField, Min(0f)] private float divingSpeedThreshold = 0.1f;
    [Header("Rotation")]
    [Tooltip("Tilt relative to the starting rotation at maximum upward speed.")]
    [SerializeField, Range(-90f, 90f)] private float risingAngle = 20f;
    [Tooltip("Tilt relative to the starting rotation at maximum falling speed.")]
    [SerializeField, Range(-90f, 90f)] private float fallingAngle = -30f;
    [Tooltip("Rotation speed in degrees per second. Angles assume the penguin faces right.")]
    [SerializeField, Min(0f)] private float rotationSpeed = 180f;
    [Header("Events")]
    [SerializeField] private UnityEvent onStarted = new UnityEvent();
    [SerializeField] private UnityEvent onDied = new UnityEvent();
    [SerializeField] private UnityEvent onReset = new UnityEvent();
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float startRotation;
    private bool flapPending;
    private bool rewardPaid;

    public long Score { get; private set; }
    public event System.Action GameReset;

    public bool HasStarted { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsRunning => isActiveAndEnabled && HasStarted && !IsDead &&
                             !GameplayPauseService.IsGameplayInputPaused && Time.timeScale > 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        startRotation = rb.rotation;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.simulated = false;
        if (playerRenderer == null) playerRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerRenderer != null && normalSprite == null) normalSprite = playerRenderer.sprite;
        SetSprite(normalSprite);
        UpdateScoreText();
        if (refreshScreen != null) refreshScreen.SetActive(false);
    }

    private void OnEnable()
    {
        GameplayPauseService.OnPauseChanged += HandlePause;
    }
    private void OnDisable()
    {
        GameplayPauseService.OnPauseChanged -= HandlePause;
        rb.simulated = false;
        flapPending = false;
    }

    private void HandlePause(bool paused)
    {
        rb.simulated = !paused && HasStarted && !IsDead && isActiveAndEnabled;
        if (paused) flapPending = false;
    }

    private void Update()
    {
        rb.simulated = IsRunning;
        if (IsDead || GameplayPauseService.IsGameplayInputPaused || Time.timeScale <= 0f) return;
        bool pressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            pressed |= IsInside(Touchscreen.current.primaryTouch.position.ReadValue());
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            pressed |= IsInside(Mouse.current.position.ReadValue());
        if (pressed) Flap();
    }

    private bool IsInside(Vector2 screenPoint) => gameZone != null && gameZone.ContainsScreenPoint(screenPoint);

    private void LateUpdate()
    {
        if (!IsRunning) return;
        bool diving = !flapPending && rb.linearVelocity.y < -divingSpeedThreshold;
        SetSprite(diving && divingSprite != null ? divingSprite : normalSprite);
    }

    private void SetSprite(Sprite sprite)
    {
        if (playerRenderer == null || sprite == null || playerRenderer.sprite == sprite) return;
        Vector2 size = playerRenderer.size;
        playerRenderer.sprite = sprite;
        // Keep the configured dimensions for the player's Tiled/Sliced renderer.
        if (playerRenderer.drawMode != SpriteDrawMode.Simple) playerRenderer.size = size;
    }

    public void Flap()
    {
        if (!isActiveAndEnabled || IsDead || GameplayPauseService.IsGameplayInputPaused || Time.timeScale <= 0f)
            return;
        if (!HasStarted)
        {
            HasStarted = true;
            rb.simulated = true;
            onStarted.Invoke();
        }
        flapPending = true;
        SetSprite(normalSprite);
    }

    private void FixedUpdate()
    {
        if (!IsRunning) return;
        rb.linearVelocity = new Vector2(0f,
            flapPending ? flapSpeed : Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        flapPending = false;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        float verticalSpeed = rb.linearVelocity.y;
        float tilt = verticalSpeed >= 0f
            ? Mathf.Lerp(0f, risingAngle, Mathf.Clamp01(verticalSpeed / Mathf.Max(0.1f, flapSpeed)))
            : Mathf.Lerp(0f, fallingAngle, Mathf.Clamp01(-verticalSpeed / Mathf.Max(0.1f, maxFallSpeed)));

        // FreezeRotation blocks collision-driven spinning; the controller sets the angle explicitly.
        rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, startRotation + tilt,
            rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (HasStarted && !IsDead && (collisionLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            IsDead = true;
            flapPending = false;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            PayReward();
            earnedText.text = scoreText.text;
            if (refreshScreen != null) refreshScreen.SetActive(true);
            onDied.Invoke();
        }
    }

    public void ResetGame()
    {
        // Retry saving before discarding a finished run if the previous save failed.
        if (IsDead) PayReward();
        rb.simulated = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = startPosition;
        rb.rotation = startRotation;
        HasStarted = false;
        IsDead = false;
        flapPending = false;
        Score = 0;
        rewardPaid = false;
        UpdateScoreText();
        earnedText.text = "0";
        if (refreshScreen != null) refreshScreen.SetActive(false);
        SetSprite(normalSprite);
        GameReset?.Invoke();
        onReset.Invoke();
    }

    public void AddPipeScore()
    {
        if (!IsRunning) return;
        if (powerValue == null || incomeValue == null)
        {
            Debug.LogError("PinguinController: assign Power Value and Income Value from the main game.", this);
            return;
        }

        long passiveReward = incomeValue.Value <= 0
            ? 5L
            : (long)incomeValue.Value * Mathf.Max(0, incomeMultiplier);
        long reward = (long)Mathf.Max(0, powerValue.Value) * Mathf.Max(0, clickMultiplier)
                    + passiveReward;
        Score += System.Math.Min(reward, long.MaxValue - Score);
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null) scoreText.text = Score.ToString();
    }

    private void PayReward()
    {
        if (rewardPaid || Score == 0) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoinsAndSave(Score);
        else
        {
            // SampleScene reloads its balance from disk, not from the value asset.
            long balance = new SaveGameSystem().AddCoins(Score);
            if (scoreValue != null) scoreValue.Value = balance;
        }
        rewardPaid = true;
    }

    public void ExitToMainScene()
    {
        if (IsDead) PayReward();
        SceneManager.LoadScene("MainScene");
    }
}
