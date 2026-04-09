using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip calmMusic;

    private void Start()
    {
        PlayCalmMusic();
    }

    public void PlayCalmMusic()
    {
        if (musicSource.clip != calmMusic)
        {
            musicSource.Stop();
            musicSource.clip = calmMusic;
            musicSource.Play();
        }
    }

    public void PlayCombatMusic()
    {
        if (musicSource.clip != combatMusic)
        {
            musicSource.Stop();
            musicSource.clip = combatMusic;
            musicSource.Play();
        }
    }
}
