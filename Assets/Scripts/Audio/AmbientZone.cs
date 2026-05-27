using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientZone : MonoBehaviour
{
    [SerializeField] private AudioClip ambientClip;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool triggerOnEnter = false;
    [Range(0f, 1f)] [SerializeField] private float volume = 0.4f;
    [SerializeField] private float fadeTime = 1.5f;

    private AudioSource _source;
    private float _targetVolume;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;
        _source.volume = 0f;
        _source.spatialBlend = 0f;
        _targetVolume = volume;
    }

    private void Start()
    {
        AudioManager.OnAmbientVolumeChanged += OnGlobalAmbientVolumeChanged;

        if (playOnAwake && ambientClip != null)
            Play();
    }

    private void OnDestroy()
    {
        AudioManager.OnAmbientVolumeChanged -= OnGlobalAmbientVolumeChanged;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnEnter && other.CompareTag("Player"))
            Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerOnEnter && other.CompareTag("Player"))
            Stop();
    }

    public void Play()
    {
        if (ambientClip == null) return;

        float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.AmbientVolume : 1f;

        if (_source.clip != ambientClip)
        {
            _source.clip = ambientClip;
            _source.Play();
        }
        else if (!_source.isPlaying)
        {
            _source.Play();
        }

        FadeTo(_targetVolume * globalVolume);
    }

    public void Stop()
    {
        FadeTo(0f, () => _source.Stop());
    }

    public void SetVolume(float newVolume)
    {
        _targetVolume = Mathf.Clamp01(newVolume);
        if (_source.isPlaying)
            FadeTo(_targetVolume);
    }

    private void FadeTo(float target, System.Action onComplete = null)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeCoroutine(target, onComplete));
    }

    private System.Collections.IEnumerator FadeCoroutine(float target, System.Action onComplete)
    {
        float start = _source.volume;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            _source.volume = Mathf.Lerp(start, target, elapsed / fadeTime);
            yield return null;
        }

        _source.volume = target;
        onComplete?.Invoke();
    }

    private void OnGlobalAmbientVolumeChanged(float globalVolume)
    {
        if (_source.isPlaying)
            _source.volume = _targetVolume * globalVolume;
    }
}
