using UnityEngine;

public static class SettingsService
{
    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(USER_PREFS_SETTINGS_KEYS.MasterVolume, 1f);
        set => PlayerPrefs.SetFloat(USER_PREFS_SETTINGS_KEYS.MasterVolume, Mathf.Clamp01(value));
    }

    public static float SfxVolume
    {
        get => PlayerPrefs.GetFloat(USER_PREFS_SETTINGS_KEYS.SfxVolume, 1f);
        set => PlayerPrefs.SetFloat(USER_PREFS_SETTINGS_KEYS.SfxVolume, Mathf.Clamp01(value));
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(USER_PREFS_SETTINGS_KEYS.MusicVolume, 1f);
        set => PlayerPrefs.SetFloat(USER_PREFS_SETTINGS_KEYS.MusicVolume, Mathf.Clamp01(value));
    }

    public static bool MusicEnabled
    {
        get => PlayerPrefs.GetInt(USER_PREFS_SETTINGS_KEYS.MusicEnabled, 1) == 1;
        set => PlayerPrefs.SetInt(USER_PREFS_SETTINGS_KEYS.MusicEnabled, value ? 1 : 0);
    }

    public static bool ScreenShakeEnabled
    {
        get => PlayerPrefs.GetInt(USER_PREFS_SETTINGS_KEYS.ScreenShakeEnabled, 1) == 1;
        set => PlayerPrefs.SetInt(USER_PREFS_SETTINGS_KEYS.ScreenShakeEnabled, value ? 1 : 0);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
        AudioManager.Instance?.ApplyVolumes();
    }
}
