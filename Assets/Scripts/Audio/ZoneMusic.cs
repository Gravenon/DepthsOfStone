using UnityEngine;

/// <summary>
/// Changes music when scene loads or player enters trigger zone.
/// For ambient sounds, use MultiAmbientZone component instead.
/// </summary>
public class ZoneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private bool triggerOnAwake = true;
    [SerializeField] private float crossfadeTime = 1.5f;

    private void Start()
    {
        if (triggerOnAwake)
            ApplyMusic();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnAwake && other.CompareTag("Player"))
            ApplyMusic();
    }

    private void ApplyMusic()
    {
        if (AudioManager.Instance != null && music != null)
            StartCoroutine(AudioManager.Instance.CrossfadeMusic(music, crossfadeTime));
    }
}
