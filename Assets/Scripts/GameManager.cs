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
        // Переназначаем менеджеры, которые могут быть уничтожены при смене сцены
        if (QuestManager == null)
            QuestManager = FindFirstObjectByType<QuestManager>();
        if (DialogueManager == null)
            DialogueManager = FindFirstObjectByType<DialogueManager>();
        if (DialogueHistoryTraker == null)
            DialogueHistoryTraker = FindFirstObjectByType<DialogueHistoryTraker>();
        if (LocationHistoryTracker == null)
            LocationHistoryTracker = FindFirstObjectByType<LocationHistoryTracker>();

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (listeners.Length <= 1) return;

        bool keptOne = false;
        foreach (AudioListener listener in listeners)
        {
            if (!keptOne)
            {
                listener.enabled = true;
                keptOne = true;
            }
            else
            {
                listener.enabled = false;
            }
        }
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
