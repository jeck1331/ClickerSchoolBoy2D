using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class WhackMole : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField, Min(0.01f)] private float riseDuration = 0.15f;
    [SerializeField, Min(0.01f)] private float hideDuration = 0.2f;
    [SerializeField, Min(0.01f)] private float hitHideDuration = 0.45f;

    private enum State { Hidden, Rising, Visible, Hiding }
    private State state;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D hitBox;
    private WhackMoleManager owner;
    private WhackMoleHole hole;
    private float elapsed;
    private float lifetime;
    private float descentDuration;

    public bool IsConfigured => normalSprite != null && hitSprite != null;
    public bool IsHidden => state == State.Hidden;

    public void Initialize(WhackMoleManager manager, WhackMoleHole targetHole)
    {
        owner = manager;
        hole = targetHole;
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<BoxCollider2D>();
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        // Fit once in world units, compensating for each cell's non-uniform scale.
        float worldScale = hole.MoleWidth / normalSprite.bounds.size.x;
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(worldScale / Mathf.Abs(parentScale.x),
            worldScale / Mathf.Abs(parentScale.y), transform.localScale.z);
        ResetMole();
    }

    public void Show(float visibleLifetime)
    {
        lifetime = visibleLifetime;
        elapsed = 0f;
        state = State.Rising;
        transform.position = hole.HiddenPosition;
        spriteRenderer.sprite = normalSprite;
        spriteRenderer.enabled = true;
        gameObject.SetActive(true);
    }

    public void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        switch (state)
        {
            case State.Rising:
                float rise = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, riseDuration));
                transform.position = Vector3.Lerp(hole.HiddenPosition, hole.VisiblePosition,
                    Mathf.SmoothStep(0f, 1f, rise));
                if (rise < 1f) return;
                state = State.Visible;
                elapsed = 0f;
                hitBox.enabled = true;
                break;
            case State.Visible:
                if (elapsed >= lifetime) BeginHiding(false);
                break;
            case State.Hiding:
                float fall = Mathf.Clamp01(elapsed / descentDuration);
                transform.position = Vector3.Lerp(hole.VisiblePosition, hole.HiddenPosition,
                    Mathf.SmoothStep(0f, 1f, fall));
                if (fall >= 1f) ResetMole();
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || state != State.Visible || !owner.IsRunning) return;
        BeginHiding(true);
        owner.RegisterHit();
    }

    private void BeginHiding(bool hit)
    {
        state = State.Hiding;
        elapsed = 0f;
        hitBox.enabled = false;
        descentDuration = Mathf.Max(0.01f, hit ? hitHideDuration : hideDuration);
        if (hit) spriteRenderer.sprite = hitSprite;
    }

    public void ResetMole()
    {
        state = State.Hidden;
        hitBox.enabled = false;
        spriteRenderer.enabled = false;
        transform.position = hole.HiddenPosition;
        gameObject.SetActive(false);
    }
}
