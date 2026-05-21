using UnityEngine;

// Place one per scene (triggerOnAwake = true) or as a trigger area (triggerOnAwake = false + Collider2D IsTrigger).
public class ZoneMusic : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip ambient;

    [SerializeField] private bool  triggerOnAwake = true;
    [SerializeField] private float crossfadeTime  = 1.5f;

    private void Start()
    {
        if (triggerOnAwake) Apply();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnAwake && other.CompareTag("Player")) Apply();
    }

    private void Apply()
    {
        if (AudioManager.Instance == null) return;
        StartCoroutine(AudioManager.Instance.CrossfadeMusic(music, crossfadeTime));
        if (ambient != null) StartCoroutine(AudioManager.Instance.CrossfadeAmbient(ambient, crossfadeTime));
        else AudioManager.Instance.StopAmbient();
    }
}
