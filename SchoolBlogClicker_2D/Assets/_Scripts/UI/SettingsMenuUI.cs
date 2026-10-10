using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle screenShakeToggle;

    private void OnEnable()
    {
        SyncFromPrefs();
    }

    public void Open()
    {
        if (panel != null)
            panel.SetActive(true);

        GameplayPauseService.SetPaused(this, true);
        SyncFromPrefs();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        GameplayPauseService.SetPaused(this, false);
    }

    private void OnDisable()
    {
        GameplayPauseService.SetPaused(this, false);
    }

    public void OnMasterVolumeChanged(float value)
    {
        SettingsService.MasterVolume = value;
        SettingsService.Save();
    }

    public void OnSfxVolumeChanged(float value)
    {
        SettingsService.SfxVolume = value;
        SettingsService.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        SettingsService.MusicVolume = value;
        SettingsService.Save();
    }

    public void OnMusicEnabledChanged(bool value)
    {
        SettingsService.MusicEnabled = value;
        SettingsService.Save();
    }

    public void OnScreenShakeChanged(bool value)
    {
        SettingsService.ScreenShakeEnabled = value;
        SettingsService.Save();
    }

    private void SyncFromPrefs()
    {
        if (masterSlider != null) masterSlider.SetValueWithoutNotify(SettingsService.MasterVolume);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(SettingsService.SfxVolume);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(SettingsService.MusicVolume);
        if (musicToggle != null) musicToggle.SetIsOnWithoutNotify(SettingsService.MusicEnabled);
        if (screenShakeToggle != null) screenShakeToggle.SetIsOnWithoutNotify(SettingsService.ScreenShakeEnabled);
    }
}
