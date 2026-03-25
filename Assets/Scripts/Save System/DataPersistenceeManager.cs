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

    // Survive scene transitions without DontDestroyOnLoad.
    private static string _activeProfilID = "";
    private static bool _pendingNewGame = false;
    private static GameData _pendingGameData = null;
    public static bool SuppressNextSave = false;

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

    // ---------------------------------------------------------------
    // Scene events
    // ---------------------------------------------------------------

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (_pendingNewGame)
        {
            gameData = _pendingGameData;
            _pendingNewGame = false;
            _pendingGameData = null;

            // Step 1: Reset all objects to clean defaults.
            ApplyDataToAll(new GameData("", ""));

            // Step 2: Apply the actual new-game data on top.
            ApplyDataToAll(gameData);

            dataHandler.Save(gameData, profilID);
        }
        else
        {
            LoadGame();
        }
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

    // ---------------------------------------------------------------
    // Public API
    // ---------------------------------------------------------------

    /// <summary>
    /// Select a profile slot. Resets all runtime state first so no
    /// data from a previous world can bleed through.
    /// </summary>
    public void ChangeSelectedProfileID(string newProfilID)
    {
        profilID = newProfilID;
        gameData = dataHandler.Load(newProfilID);

        // Always do a full reset first — wipes any in-memory leftovers
        // from a previously active world (time, inventory, stats, etc.).
        HardResetAllObjects();

        if (gameData != null)
        {
            // Apply this profile's data on top of the clean state.
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            ApplyDataToAll(gameData);
        }
    }

    public void NewGame(string worldName, string playerName)
    {
        _pendingGameData = new GameData(worldName, playerName);
        _pendingNewGame = true;

        // Wipe any leftover runtime state immediately (operates on live
        // objects — DDOL ones like WorldTime and InventoryManager).
        HardResetAllObjects();

        if (WorldTime.Instance != null)
            WorldTime.Instance.ResetToMorning();

        // Write clean file so the slot shows data in the menu immediately.
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

    /// <summary>True when the currently selected profile has a save file on disk.</summary>
    public bool HasActiveGameData => gameData != null;

    public Dictionary<string, GameData> GetAllProfilesGameData()
        => dataHandler.LoadAllProfiles();

    /// <summary>
    /// Returns the scene where the last checkpoint was saved.
    /// Falls back to the active scene.
    /// </summary>
    public string GetLastSavedScene()
    {
        if (gameData != null && !string.IsNullOrEmpty(gameData.lastScene))
            return gameData.lastScene;
        return SceneManager.GetActiveScene().name;
    }

    // ---------------------------------------------------------------
    // Internal helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Hard-resets every IDataPersistence object currently alive in
    /// any scene (including DontDestroyOnLoad) by feeding each a blank
    /// GameData. Re-discovers objects fresh every call so the list is
    /// never stale.
    /// </summary>
    private void HardResetAllObjects()
    {
        if (WorldTime.Instance != null)
            WorldTime.Instance.ResetToMorning();

        GameData blank = new GameData("", "");
        List<IDataPersistence> all = FindAllDataPersistenceObjects();
        foreach (IDataPersistence obj in all)
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
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
               .OfType<IDataPersistence>()
               .ToList();
    }
}

