using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceeManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string dataDirName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FilePathHandler dataHandler;

    // Static fields persist across scene transitions without DontDestroyOnLoad.
    private static string _activeProfilID = "";
    private static bool _pendingNewGame = false;
    private static GameData _pendingGameData = null;
    public static bool SuppressNextSave = false;
    public static bool IsRespawning = false;

    private string profilID
    {
        get => _activeProfilID;
        set => _activeProfilID = value;
    }

    public static DataPersistenceeManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        dataHandler = new FilePathHandler(Application.persistentDataPath, dataDirName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (_pendingNewGame)
        {
            gameData = _pendingGameData;
            _pendingNewGame = false;
            _pendingGameData = null;

            ApplyDataToAll(new GameData("", ""));
            ApplyDataToAll(gameData);
            dataHandler.Save(gameData, profilID);
        }
        else
        {
            LoadGame();
        }

        IsRespawning = false;
    }

    public void OnSceneUnloaded(Scene scene)
    {
        if (_pendingNewGame || SuppressNextSave)
        {
            SuppressNextSave = false;
            return;
        }
        SaveGame();
    }

    public void ChangeSelectedProfileID(string newProfilID)
    {
        profilID = newProfilID;
        gameData = dataHandler.Load(newProfilID);

        HardResetAllObjects();

        if (gameData != null)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            ApplyDataToAll(gameData);
        }
    }

    public void NewGame(string worldName, string playerName)
    {
        _pendingGameData = new GameData(worldName, playerName);
        _pendingNewGame = true;

        HardResetAllObjects();

        if (WorldTime.Instance != null)
            WorldTime.Instance.ResetToMorning();

        dataHandler.Save(_pendingGameData, profilID);
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load(profilID);

        if (gameData == null)
        {
            Debug.LogWarning("[DPM] No save file for profile: " + profilID);
            return;
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();
        ApplyDataToAll(gameData);
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.LogWarning("[DPM] SaveGame skipped — no active game data.");
            return;
        }

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData, profilID);
    }

    public void OnApplicationQuit() => SaveGame();

    public void DeleteProfile(string profileID)
    {
        dataHandler.Delete(profileID);
        if (profilID == profileID)
        {
            gameData = null;
            profilID = "";
        }
    }

    public bool HasActiveGameData => gameData != null;

    /// <summary>
    /// Returns true if the current profile has never seen the intro sequence.
    /// </summary>
    public bool IsFirstTime => gameData != null && !gameData.introPlayed;

    /// <summary>
    /// Call this once the intro has finished so it won't play again for this profile.
    /// </summary>
    public void MarkIntroPlayed()
    {
        if (gameData == null) return;
        gameData.introPlayed = true;
        SaveGame();
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
        => dataHandler.LoadAllProfiles();

    public void SaveCheckpoint(Vector3 checkpointPosition)
    {
        if (gameData == null)
        {
            Debug.LogWarning("[DPM] SaveCheckpoint skipped — no active game data.");
            return;
        }

        gameData.lastScene = SceneManager.GetActiveScene().name;
        gameData.playerPosition = checkpointPosition;
        gameData.checkpointScene = SceneManager.GetActiveScene().name;
        gameData.checkpointPosition = checkpointPosition;
        gameData.checkpointHealth = StatsManager.Instance != null ? StatsManager.Instance.maxHealth : gameData.maxHealth;

        SaveGame();
        Debug.Log($"[DPM] Checkpoint saved — scene: {gameData.lastScene}, pos: {checkpointPosition}");
    }

    public string GetLastSavedScene()
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastScene))
            return gameData.lastScene;
        return SceneManager.GetActiveScene().name;
    }

    public string GetCheckpointScene()
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.checkpointScene))
            return gameData.checkpointScene;
        return GetLastSavedScene();
    }

    public bool HasCheckpoint()
        => gameData != null && !string.IsNullOrEmpty(gameData.checkpointScene);

    private void HardResetAllObjects()
    {
        if (WorldTime.Instance != null)
            WorldTime.Instance.ResetToMorning();

        GameData blank = new GameData("", "");
        foreach (IDataPersistence obj in FindAllDataPersistenceObjects())
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.LoadData(blank);
        }
    }

    private void ApplyDataToAll(GameData data)
    {
        if (dataPersistenceObjects == null) return;
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.LoadData(data);
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsInactive.Include is required so that objects on inactive GameObjects
        // (e.g. InventoryManager when the panel is closed) are found and saved correctly.
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
               .OfType<IDataPersistence>()
               .ToList();
    }
}
