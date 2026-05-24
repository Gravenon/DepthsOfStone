using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public DialogueManager DialogueManager;
    public DialogueHistoryTraker DialogueHistoryTraker;
    public LocationHistoryTracker LocationHistoryTracker;
    public QuestManager QuestManager;

    [Header("Persitent Objects")]
    public GameObject[] persistentObjects;

    private void Awake()
    {
        if(Instance != null) 
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (QuestManager == null)
            QuestManager = FindFirstObjectByType<QuestManager>();
        if (DialogueManager == null)
            DialogueManager = FindFirstObjectByType<DialogueManager>();
        if (DialogueHistoryTraker == null)
            DialogueHistoryTraker = FindFirstObjectByType<DialogueHistoryTraker>();
        if (LocationHistoryTracker == null)
            LocationHistoryTracker = FindFirstObjectByType<LocationHistoryTracker>();

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        AudioListener preferred = null;
        if (AudioManager.Instance != null)
            preferred = AudioManager.Instance.GetComponent<AudioListener>();

        foreach (AudioListener listener in listeners)
            listener.enabled = false;

        if (preferred != null)
            preferred.enabled = true;
        else if (listeners.Length > 0)
            listeners[0].enabled = true;
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if(obj != null)
                DontDestroyOnLoad(obj);
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObjects)
            Destroy(obj);

        Destroy(gameObject);
    }
}
