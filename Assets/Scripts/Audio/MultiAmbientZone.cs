using UnityEngine;

public class MultiAmbientZone : MonoBehaviour
{
    [System.Serializable]
    public class AmbientLayer
    {
        public string name = "Layer";
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.4f;
        [HideInInspector] public AudioSource source;
    }

    [SerializeField] private AmbientLayer[] layers;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool triggerOnEnter = false;
    [SerializeField] private float fadeTime = 1.5f;

    private void Awake()
    {
        foreach (var layer in layers)
        {
            if (layer.clip == null) continue;

            GameObject layerObj = new GameObject($"Ambient_{layer.name}");
            layerObj.transform.SetParent(transform);

            layer.source = layerObj.AddComponent<AudioSource>();
            layer.source.clip = layer.clip;
            layer.source.loop = true;
            layer.source.playOnAwake = false;
            layer.source.volume = 0f;
            layer.source.spatialBlend = 0f;
        }
    }

    private void Start()
    {
        AudioManager.OnAmbientVolumeChanged += OnGlobalAmbientVolumeChanged;

        if (playOnAwake)
            Play();
    }

    private void OnDestroy()
    {
        AudioManager.OnAmbientVolumeChanged -= OnGlobalAmbientVolumeChanged;

        foreach (var layer in layers)
        {
            if (layer.source != null)
                Destroy(layer.source.gameObject);
        }
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
        float globalVolume = AudioManager.Instance != null ? AudioManager.Instance.AmbientVolume : 1f;

        foreach (var layer in layers)
        {
            if (layer.source != null && layer.clip != null)
            {
                if (!layer.source.isPlaying)
                    layer.source.Play();
                StartCoroutine(FadeSource(layer.source, layer.source.volume, layer.volume * globalVolume, fadeTime));
            }
        }
    }

    public void Stop()
    {
        foreach (var layer in layers)
        {
            if (layer.source != null)
            {
                StartCoroutine(FadeSource(layer.source, layer.source.volume, 0f, fadeTime, () => layer.source.Stop()));
            }
        }
    }

    public void SetLayerVolume(int index, float volume)
    {
        if (index >= 0 && index < layers.Length && layers[index].source != null)
        {
            layers[index].volume = Mathf.Clamp01(volume);
            StartCoroutine(FadeSource(layers[index].source, layers[index].source.volume, layers[index].volume, fadeTime));
        }
    }

    public void SetLayerVolume(string layerName, float volume)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
            {
                SetLayerVolume(i, volume);
                break;
            }
        }
    }

    // Called when the global ambient volume slider changes.
    private void OnGlobalAmbientVolumeChanged(float globalVolume)
    {
        foreach (var layer in layers)
        {
            if (layer.source != null && layer.source.isPlaying)
                layer.source.volume = layer.volume * globalVolume;
        }
    }

    private System.Collections.IEnumerator FadeSource(AudioSource source, float from, float to, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        source.volume = to;
        onComplete?.Invoke();
    }
}
