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
            // Sources can be separate objects, so destroying just the manager is insufficient.
            if (musicSource != null && musicSource != Instance.musicSource)
            {
                musicSource.playOnAwake = false;
                musicSource.Stop();
            }
            if (sfxSource != null && sfxSource != Instance.sfxSource)
            {
                sfxSource.playOnAwake = false;
                sfxSource.Stop();
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Silence old sources left on the manager after moving audio to separate objects.
        foreach (var source in GetComponents<AudioSource>())
        {
            if (source == musicSource || source == sfxSource) continue;
            source.Stop();
            source.playOnAwake = false;
            source.enabled = false;
        }

        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.Stop();
            musicSource.loop = true;
            if (defaultMusicTrack != null) musicSource.clip = defaultMusicTrack;
            if (musicSource.transform != transform && !musicSource.transform.IsChildOf(transform))
                musicSource.transform.SetParent(transform, true);
        }
        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.Stop();
            sfxSource.clip = null;
            sfxSource.loop = false;
            if (sfxSource.transform != transform && !sfxSource.transform.IsChildOf(transform))
                sfxSource.transform.SetParent(transform, true);
        }

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
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        if (musicSource == null || clip == null) return;
        musicSource.loop = loop;
        if (musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
        }
        if (SettingsService.MusicEnabled && !musicSource.isPlaying)
            musicSource.Play();
        else if (!SettingsService.MusicEnabled)
            musicSource.Stop();
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

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
