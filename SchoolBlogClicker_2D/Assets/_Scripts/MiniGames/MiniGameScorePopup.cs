using TMPro;
using UnityEngine;

internal sealed class MiniGameScorePopup
{
    public const float DefaultDuration = 0.7f;
    public const float DefaultRise = 100f;

    private readonly RectTransform parent;
    private readonly Camera uiCamera;
    private readonly TextMeshProUGUI label;
    private readonly RectTransform rect;
    private Vector2 origin;
    private float elapsed;

    public bool IsVisible => label != null && label.gameObject.activeSelf;

    public MiniGameScorePopup(TMP_Text scoreLabel)
    {
        Canvas canvas = scoreLabel.canvas;
        parent = (RectTransform)canvas.transform;
        Canvas root = canvas.rootCanvas;
        uiCamera = root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;

        GameObject popup = new GameObject("Score +1", typeof(RectTransform), typeof(TextMeshProUGUI));
        popup.SetActive(false);
        popup.transform.SetParent(parent, false);
        label = popup.GetComponent<TextMeshProUGUI>();
        label.font = scoreLabel.font;
        label.fontSharedMaterial = scoreLabel.fontSharedMaterial;
        label.fontSize = 48f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        label.text = "+1";
        rect = label.rectTransform;
        rect.sizeDelta = new Vector2(120f, 80f);
    }

    public void Show(Vector2 screenPosition)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPosition, uiCamera, out origin)) return;
        elapsed = 0f;
        rect.localPosition = origin;
        label.alpha = 1f;
        label.gameObject.SetActive(true);
    }

    public void Tick(float deltaTime, float duration = DefaultDuration, float rise = DefaultRise)
    {
        if (!IsVisible) return;
        elapsed += deltaTime;
        float progress = Mathf.Clamp01(elapsed / duration);
        rect.localPosition = origin + Vector2.up * (rise * progress);
        label.alpha = 1f - progress;
        if (progress >= 1f) Hide();
    }

    public void Hide()
    {
        if (IsVisible) label.gameObject.SetActive(false);
    }

    public void Destroy()
    {
        if (label != null) Object.Destroy(label.gameObject);
    }
}
