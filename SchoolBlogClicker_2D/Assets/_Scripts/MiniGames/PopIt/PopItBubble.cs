using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CircleCollider2D))]
public class PopItBubble : MonoBehaviour, IPointerDownHandler
{
    private PopItManager owner;
    private GameObject normalState;
    private GameObject pressedState;
    private float recoveryRemaining;
    private MiniGameScorePopup popup;

    public bool IsPressed { get; private set; }
    public Vector3 Center => normalState.GetComponent<SpriteRenderer>().bounds.center;

    public void Initialize(PopItManager manager, TMP_Text scoreLabel)
    {
        owner = manager;
        normalState = transform.GetChild(0).gameObject;
        pressedState = transform.GetChild(1).gameObject;
        SpriteRenderer sprite = normalState.GetComponent<SpriteRenderer>();
        CircleCollider2D hitArea = GetComponent<CircleCollider2D>();
        hitArea.offset = transform.InverseTransformPoint(sprite.bounds.center);
        hitArea.radius = Mathf.Min(sprite.bounds.size.x / Mathf.Abs(transform.lossyScale.x),
            sprite.bounds.size.y / Mathf.Abs(transform.lossyScale.y)) * 0.5f;

        if (scoreLabel != null && scoreLabel.canvas != null)
            popup = new MiniGameScorePopup(scoreLabel);
        ResetBubble();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || IsPressed || !owner.IsRunning) return;
        IsPressed = true;
        recoveryRemaining = owner.GetRecoveryDelay();
        normalState.SetActive(false);
        pressedState.SetActive(true);
        owner.RegisterPress();
        popup?.Show(RectTransformUtility.WorldToScreenPoint(owner.WorldCamera, Center));
    }

    public bool Tick(float deltaTime, float recoverySpeed)
    {
        popup?.Tick(deltaTime, owner.PopupDuration, owner.PopupRise);
        if (!IsPressed) return false;
        recoveryRemaining -= deltaTime * recoverySpeed;
        if (recoveryRemaining > 0f) return false;
        IsPressed = false;
        normalState.SetActive(true);
        pressedState.SetActive(false);
        return true;
    }

    public void ResetBubble()
    {
        IsPressed = false;
        recoveryRemaining = 0f;
        normalState.SetActive(true);
        pressedState.SetActive(false);
        HidePopup();
    }

    public void HidePopup() => popup?.Hide();

    private void OnDestroy() => popup?.Destroy();
}
