using UnityEngine;

// Attach to Player, Enemy or NPC alongside a local AudioSource.
// Sounds play from the entity's position; multiple sounds overlap via PlayOneShot.
public class AudiManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip[] footstepSounds;

    [SerializeField] private float footstepCooldown = 0.35f;
    private float _footstepTimer;

    [Header("Ambient")]
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioClip ambientClip;

    public void PlayDashSound() => PlayWithPitch(dashSound);
    public void PlayHitSound()  => PlayWithPitch(hitSound);

    private void Start()
    {
        if (ambientAudioSource != null && ambientClip != null)
        {
            ambientAudioSource.clip = ambientClip;
            ambientAudioSource.loop = true;
            ambientAudioSource.volume = AudioManager.Instance != null ? AudioManager.Instance.SfxVolume : 1f;
            ambientAudioSource.Play();
        }
    }

    public void PlayAmbient()
    {
        if (ambientAudioSource == null || ambientClip == null) return;
        if (ambientAudioSource.isPlaying) return;
        ambientAudioSource.volume = AudioManager.Instance != null ? AudioManager.Instance.SfxVolume : 1f;
        ambientAudioSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientAudioSource != null && ambientAudioSource.isPlaying)
            ambientAudioSource.Stop();
    }


    // Call every frame while the entity is moving.
    public void TickFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        _footstepTimer -= Time.deltaTime;
        if (_footstepTimer > 0f) return;
        _footstepTimer = footstepCooldown;
        PlayWithPitch(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    private void PlayWithPitch(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        float volume = AudioManager.Instance != null ? AudioManager.Instance.SfxVolume : 1f;
        audioSource.PlayOneShot(clip, volume);
    }
}
