using System.Collections;
using UnityEngine;

// Persistent singleton — place on a GameObject in the Menu scene ONLY.
// Owns the single AudioListener. Provides Music, SFX and Ambient channels.
// Remove AudioListener from ALL cameras in ALL scenes.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusic; // plays automatically when the game starts

    [Header("Default Volumes")]
    [Range(0f, 1f)] [SerializeField] private float defaultMusicVolume   = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float defaultSfxVolume     = 1.0f;
    [Range(0f, 1f)] [SerializeField] private float defaultAmbientVolume = 0.4f;

    private const string KeyMusic = "Vol_Music";
    private const string KeySfx = "Vol_SFX";
    private const string KeyAmbient = "Vol_Ambient";

    private float _musicTargetVolume;
    private float _sfxTargetVolume;
    private float _ambientTargetVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumes();
        PlayMusic(menuMusic);
    }

    // Music

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return; // skip only if already audibly playing
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = _musicTargetVolume;
        musicSource.Play();
    }

    public IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
    {
        if (newClip == null || musicSource.clip == newClip) yield break;
        yield return FadeSource(musicSource, musicSource.volume, 0f, duration * 0.5f);
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
        yield return FadeSource(musicSource, 0f, _musicTargetVolume, duration * 0.5f);
    }

    public void StopMusic() => musicSource.Stop();

    // SFX — for UI or generic sounds; per-character sounds go through AudiManager

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxSource.volume);
    }

    // Ambient

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource.clip == clip) return;
        ambientSource.Stop();
        ambientSource.clip   = clip;
        ambientSource.loop   = true;
        ambientSource.volume = _ambientTargetVolume;
        ambientSource.Play();
    }

    public IEnumerator CrossfadeAmbient(AudioClip newClip, float duration)
    {
        if (newClip == null || ambientSource.clip == newClip) yield break;
        yield return FadeSource(ambientSource, ambientSource.volume, 0f, duration * 0.5f);
        ambientSource.clip = newClip;
        ambientSource.loop = true;
        ambientSource.Play();
        yield return FadeSource(ambientSource, 0f, _ambientTargetVolume, duration * 0.5f);
    }

    public void StopAmbient() => StartCoroutine(FadeSource(ambientSource, ambientSource.volume, 0f, 0.5f));

    // Volume properties — values are saved to PlayerPrefs automatically

    public float MusicVolume
    {
        get => _musicTargetVolume;
        set { _musicTargetVolume = Mathf.Clamp01(value); musicSource.volume = _musicTargetVolume; PlayerPrefs.SetFloat(KeyMusic, _musicTargetVolume); }
    }

    public float SfxVolume
    {
        get => _sfxTargetVolume;
        set { _sfxTargetVolume = Mathf.Clamp01(value); sfxSource.volume = _sfxTargetVolume; PlayerPrefs.SetFloat(KeySfx, _sfxTargetVolume); }
    }

    // Call from a "Reset to defaults" button if volumes get stuck.
    public void ResetVolumesToDefault()
    {
        PlayerPrefs.DeleteKey(KeyMusic);
        PlayerPrefs.DeleteKey(KeySfx);
        PlayerPrefs.DeleteKey(KeyAmbient);
        LoadVolumes();
        Debug.Log("[AudioManager] Volumes reset to defaults.");
    }

    public float AmbientVolume
    {
        get => _ambientTargetVolume;
        set { _ambientTargetVolume = Mathf.Clamp01(value); ambientSource.volume = _ambientTargetVolume; PlayerPrefs.SetFloat(KeyAmbient, _ambientTargetVolume); }
    }

    private void LoadVolumes()
    {
        _musicTargetVolume   = PlayerPrefs.GetFloat(KeyMusic,   defaultMusicVolume);
        _sfxTargetVolume     = PlayerPrefs.GetFloat(KeySfx,     defaultSfxVolume);
        _ambientTargetVolume = PlayerPrefs.GetFloat(KeyAmbient, defaultAmbientVolume);
        musicSource.volume   = _musicTargetVolume;
        sfxSource.volume     = _sfxTargetVolume;
        ambientSource.volume = _ambientTargetVolume;
    }

    private IEnumerator FadeSource(AudioSource source, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        source.volume = to;
    }
}
