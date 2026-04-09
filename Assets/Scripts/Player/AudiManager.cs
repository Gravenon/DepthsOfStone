using UnityEngine;

public class AudiManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip hitSound;


    public void PlayDashSound()
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(dashSound);
    }

    public void PlayHitSound()
    {
        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(hitSound);
    }  
}
