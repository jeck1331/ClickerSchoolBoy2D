using System;
using System.Collections.Generic;
using UnityEngine;

public enum SfxType
{
    Click,
    Crit,
    MiniHit,
    MiniMiss,
    UpgradeBuy
}

[Serializable]
public struct SfxEntry
{
    public SfxType type;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip defaultMusicTrack;
    [SerializeField] private SfxEntry[] sfxEntries;

    private readonly Dictionary<SfxType, AudioClip> _sfxMap = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sfxMap.Clear();
        foreach (var entry in sfxEntries)
        {
            if (entry.clip != null)
                _sfxMap[entry.type] = entry.clip;
        }

        ApplyVolumes();
    }

    private void Start()
    {
        if (defaultMusicTrack != null)
            PlayMusic(defaultMusicTrack, true);
    }

    public void PlaySfx(SfxType type)
    {
        if (sfxSource == null) return;
        if (!_sfxMap.TryGetValue(type, out var clip) || clip == null) return;
        sfxSource.PlayOneShot(clip, SettingsService.SfxVolume);
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        if (musicSource == null || clip == null) return;
        musicSource.loop = loop;
        musicSource.clip = clip;
        if (SettingsService.MusicEnabled)
            musicSource.Play();
    }

    public void ApplyVolumes()
    {
        var master = SettingsService.MasterVolume;
        if (sfxSource != null) sfxSource.volume = master * SettingsService.SfxVolume;
        if (musicSource != null) musicSource.volume = master * SettingsService.MusicVolume;
        if (musicSource != null && !SettingsService.MusicEnabled && musicSource.isPlaying) musicSource.Stop();
        if (musicSource != null && SettingsService.MusicEnabled && !musicSource.isPlaying && musicSource.clip != null)
            musicSource.Play();
    }
}
