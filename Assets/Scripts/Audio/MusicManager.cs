using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip calmMusic;
    [SerializeField] private AudioClip combatMusic;

    private void Awake() => Instance = this;
    private void Start() => PlayCalmMusic();

    public void PlayCalmMusic()
    {
        if (AudioManager.Instance != null && calmMusic != null)
            AudioManager.Instance.PlayMusic(calmMusic);
    }

    public void PlayCombatMusic()
    {
        if (AudioManager.Instance != null && combatMusic != null)
            AudioManager.Instance.PlayMusic(combatMusic);
    }
}
