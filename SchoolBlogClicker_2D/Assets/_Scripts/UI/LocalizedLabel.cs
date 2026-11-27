using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public sealed class LocalizedLabel : MonoBehaviour
{
    [SerializeField] private string russian;
    [SerializeField] private TMP_FontAsset russianFont;

    [SerializeField] private float russianFontSize;

    private TMP_Text label;
    private string english;
    private TMP_FontAsset englishFont;
    private Material englishMaterial;
    private float englishFontSize;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
        english = label.text;
        englishFont = label.font;
        englishMaterial = label.fontSharedMaterial;
        englishFontSize = label.fontSize;
    }

    private void OnEnable()
    {
        SettingsService.LanguageChanged += Refresh;
        Refresh();
    }

    private void OnDisable() => SettingsService.LanguageChanged -= Refresh;

    private void Refresh()
    {
        bool isRussian = SettingsService.Language == GameLanguage.Russian;
        label.font = isRussian && russianFont != null ? russianFont : englishFont;
        label.fontSharedMaterial = isRussian && russianFont != null ? russianFont.material : englishMaterial;
        label.fontSize = isRussian && russianFontSize > 0 ? russianFontSize : englishFontSize;
        label.text = isRussian ? russian : english;
    }
}
