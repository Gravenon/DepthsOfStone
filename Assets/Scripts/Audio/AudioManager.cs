using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // Fired whenever ambient volume changes so all active MultiAmbientZone components can update.
    public static event System.Action<float> OnAmbientVolumeChanged;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusic;

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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureAudioListener();
        LoadVolumes();
        PlayMusic(menuMusic);
    }

    private void OnEnable() => EnsureAudioListener();

    private void EnsureAudioListener()
    {
        AudioListener listener = GetComponent<AudioListener>();
        if (listener == null)
            listener = gameObject.AddComponent<AudioListener>();
        listener.enabled = true;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

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

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, _sfxTargetVolume);
    }

    public float MusicVolume
    {
        get => _musicTargetVolume;
        set
        {
            _musicTargetVolume = Mathf.Clamp01(value);
            musicSource.volume = _musicTargetVolume;
            PlayerPrefs.SetFloat(KeyMusic, _musicTargetVolume);
        }
    }

    public float SfxVolume
    {
        get => _sfxTargetVolume;
        set
        {
            _sfxTargetVolume = Mathf.Clamp01(value);
            sfxSource.volume = _sfxTargetVolume;
            PlayerPrefs.SetFloat(KeySfx, _sfxTargetVolume);
        }
    }

    public float AmbientVolume
    {
        get => _ambientTargetVolume;
        set
        {
            _ambientTargetVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KeyAmbient, _ambientTargetVolume);
            OnAmbientVolumeChanged?.Invoke(_ambientTargetVolume);
        }
    }

    public void ResetVolumesToDefault()
    {
        PlayerPrefs.DeleteKey(KeyMusic);
        PlayerPrefs.DeleteKey(KeySfx);
        PlayerPrefs.DeleteKey(KeyAmbient);
        LoadVolumes();
    }

    private void LoadVolumes()
    {
        _musicTargetVolume = PlayerPrefs.GetFloat(KeyMusic,   defaultMusicVolume);
        _sfxTargetVolume = PlayerPrefs.GetFloat(KeySfx,     defaultSfxVolume);
        _ambientTargetVolume = PlayerPrefs.GetFloat(KeyAmbient, defaultAmbientVolume);
        musicSource.volume = _musicTargetVolume;
        sfxSource.volume = _sfxTargetVolume;
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
