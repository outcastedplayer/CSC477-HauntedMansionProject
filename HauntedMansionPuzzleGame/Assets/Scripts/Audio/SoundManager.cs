using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Music Clips")]
    public AudioClip explorationMusic;
    public AudioClip urgencyMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;

    [Header("Ambient Clips")]
    public AudioClip ambLobby;
    public AudioClip ambOffice;
    public AudioClip ambLibrary;
    public AudioClip ambParlor;

    [Header("SFX Clips")]
    public SFXEntry[] sfxClips;

    [Header("Settings")]
    public float crossfadeDuration = 1.5f;
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float ambienceVolume = 0.7f;

    Dictionary<string, AudioClip> sfxLookup = new Dictionary<string, AudioClip>();
    Coroutine crossfadeRoutine;
    Coroutine musicCrossfadeRoutine;

    [Serializable]
    public struct SFXEntry
    {
        public string name;
        public AudioClip clip;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in sfxClips)
        {
            if (!string.IsNullOrEmpty(entry.name) && entry.clip != null)
                sfxLookup[entry.name] = entry.clip;
        }
    }

    void Start()
    {
        PlayMusic(explorationMusic);
        SetAmbience(RoomType.Lobby);
    }

    // ── SFX ──────────────────────────────────────────────

    public void PlaySFX(string clipName)
    {
        if (sfxLookup.TryGetValue(clipName, out AudioClip clip))
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlaySFXAtPoint(string clipName, Vector3 position)
    {
        if (sfxLookup.TryGetValue(clipName, out AudioClip clip))
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume * masterVolume);
    }

    public void PlayUISFX(string clipName)
    {
        if (sfxLookup.TryGetValue(clipName, out AudioClip clip))
            uiSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    // ── Music ────────────────────────────────────────────

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || !musicSource) return;
        musicSource.clip = clip;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void CrossfadeMusic(AudioClip newClip, float duration = -1f)
    {
        if (newClip == null || !musicSource) return;
        if (musicSource.clip == newClip) return;
        if (musicCrossfadeRoutine != null) StopCoroutine(musicCrossfadeRoutine);
        musicCrossfadeRoutine = StartCoroutine(CrossfadeMusicRoutine(newClip, duration > 0 ? duration : crossfadeDuration));
    }

    IEnumerator CrossfadeMusicRoutine(AudioClip newClip, float duration)
    {
        float startVol = musicSource.volume;
        float halfDuration = duration * 0.5f;

        // Fade out
        float t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / halfDuration);
            yield return null;
        }

        // Swap clip
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        float targetVol = musicVolume * masterVolume;
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVol, t / halfDuration);
            yield return null;
        }
        musicSource.volume = targetVol;
    }

    public void StopMusic(float fadeOut = 1f)
    {
        if (musicCrossfadeRoutine != null) StopCoroutine(musicCrossfadeRoutine);
        musicCrossfadeRoutine = StartCoroutine(FadeOutSource(musicSource, fadeOut));
    }

    // ── Ambience ─────────────────────────────────────────

    public void SetAmbience(RoomType room)
    {
        AudioClip clip;
        switch (room)
        {
            case RoomType.Office:  clip = ambOffice;  break;
            case RoomType.Library: clip = ambLibrary; break;
            case RoomType.Parlor:  clip = ambParlor;  break;
            default:               clip = ambLobby;   break;
        }
        CrossfadeAmbience(clip);
    }

    public void CrossfadeAmbience(AudioClip newClip)
    {
        if (newClip == null || !ambientSource) return;
        if (ambientSource.clip == newClip && ambientSource.isPlaying) return;
        if (crossfadeRoutine != null) StopCoroutine(crossfadeRoutine);
        crossfadeRoutine = StartCoroutine(CrossfadeAmbienceRoutine(newClip));
    }

    IEnumerator CrossfadeAmbienceRoutine(AudioClip newClip)
    {
        float startVol = ambientSource.volume;
        float half = crossfadeDuration * 0.5f;

        float t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(startVol, 0f, t / half);
            yield return null;
        }

        ambientSource.clip = newClip;
        ambientSource.loop = true;
        ambientSource.Play();

        float targetVol = ambienceVolume * masterVolume;
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(0f, targetVol, t / half);
            yield return null;
        }
        ambientSource.volume = targetVol;
    }

    // ── Volume ───────────────────────────────────────────

    public void SetMasterVolume(float vol)
    {
        masterVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    public void SetSFXVolume(float vol) { sfxVolume = Mathf.Clamp01(vol); }
    public void SetMusicVolume(float vol) { musicVolume = Mathf.Clamp01(vol); ApplyVolumes(); }
    public void SetAmbienceVolume(float vol) { ambienceVolume = Mathf.Clamp01(vol); ApplyVolumes(); }

    void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume * masterVolume;
        if (ambientSource) ambientSource.volume = ambienceVolume * masterVolume;
    }

    // ── Utility ──────────────────────────────────────────

    IEnumerator FadeOutSource(AudioSource source, float duration)
    {
        float startVol = source.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        source.Stop();
        source.volume = startVol;
    }
}
