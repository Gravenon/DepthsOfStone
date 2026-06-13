using UnityEngine;

public class AudiManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepCooldown = 0.35f;

    private float _footstepTimer;

    public void PlayDashSound() => PlayWithPitch(dashSound);
    public void PlayHitSound()  => PlayWithPitch(hitSound);

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
