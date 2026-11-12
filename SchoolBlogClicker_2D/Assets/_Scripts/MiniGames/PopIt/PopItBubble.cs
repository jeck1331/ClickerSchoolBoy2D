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
    private TMP_Text popup;
    private RectTransform popupParent;
    private Vector2 popupOrigin;
    private float popupElapsed;

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

        if (scoreLabel != null)
        {
            Canvas canvas = scoreLabel.canvas;
            popupParent = canvas.transform as RectTransform;
            GameObject label = new GameObject("PopIt +1", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(popupParent, false);
            popup = label.GetComponent<TextMeshProUGUI>();
            popup.font = scoreLabel.font;
            popup.fontSharedMaterial = scoreLabel.fontSharedMaterial;
            popup.fontSize = 48f;
            popup.alignment = TextAlignmentOptions.Center;
            popup.raycastTarget = false;
            popup.text = "+1";
            popup.rectTransform.sizeDelta = new Vector2(120f, 80f);
        }
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
        ShowPopup();
    }

    public bool Tick(float deltaTime, float recoverySpeed)
    {
        if (popup != null && popup.gameObject.activeSelf)
        {
            popupElapsed += deltaTime;
            float progress = Mathf.Clamp01(popupElapsed / owner.PopupDuration);
            popup.rectTransform.anchoredPosition = popupOrigin + Vector2.up * (owner.PopupRise * progress);
            popup.alpha = 1f - progress;
            if (progress >= 1f) popup.gameObject.SetActive(false);
        }
        if (!IsPressed) return false;
        recoveryRemaining -= deltaTime * recoverySpeed;
        if (recoveryRemaining > 0f) return false;
        IsPressed = false;
        normalState.SetActive(true);
        pressedState.SetActive(false);
        return true;
    }

    private void ShowPopup()
    {
        if (popup == null) return;
        Canvas canvas = popup.canvas;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(owner.WorldCamera, Center);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(popupParent, screen, uiCamera, out popupOrigin);
        popupElapsed = 0f;
        popup.rectTransform.anchoredPosition = popupOrigin;
        popup.alpha = 1f;
        popup.gameObject.SetActive(true);
    }

    public void ResetBubble()
    {
        IsPressed = false;
        recoveryRemaining = 0f;
        normalState.SetActive(true);
        pressedState.SetActive(false);
        HidePopup();
    }

    public void HidePopup()
    {
        if (popup != null) popup.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (popup != null) Destroy(popup.gameObject);
    }
}
