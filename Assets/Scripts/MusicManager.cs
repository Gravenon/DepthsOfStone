using UnityEngine;

// Scene-level combat music switcher.
// Call PlayCombatMusic() from WaveSpawner/enemy AI, PlayCalmMusic() when combat ends.
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip calmMusic;
    [SerializeField] private AudioClip combatMusic;

    private void Awake() => Instance = this;
    private void Start()  => PlayCalmMusic();

    public void PlayCalmMusic()   => AudioManager.Instance?.PlayMusic(calmMusic);
    public void PlayCombatMusic() => AudioManager.Instance?.PlayMusic(combatMusic);
}
