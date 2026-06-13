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

    private static string _activeProfilID  = "";
    private static bool _pendingNewGame  = false;
    private static GameData _pendingGameData = null;

    public static bool SuppressNextSave = false;
    public static bool IsRespawning = false;
    public static int  RespawnMaxHealth = 0;

    private string profilID { get => _activeProfilID; set => _activeProfilID = value; }

    public static DataPersistenceeManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
        else LoadGame();

        IsRespawning = false;
    }

    public void OnSceneUnloaded(Scene scene)
    {
        if (_pendingNewGame || SuppressNextSave) { SuppressNextSave = false; return; }
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
        _pendingNewGame  = true;
        HardResetAllObjects();
        if (WorldTime.Instance != null) WorldTime.Instance.ResetToMorning();
        dataHandler.Save(_pendingGameData, profilID);
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load(profilID);
        if (gameData == null) return;
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        ApplyDataToAll(gameData);
    }

    public void SaveGame()
    {
        if (gameData == null) return;
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (var obj in dataPersistenceObjects)
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData, profilID);
        SaveIndicator.Instance?.Show();
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
    public bool IsFirstTime => gameData != null && !gameData.introPlayed;

    public void MarkIntroPlayed()
    {
        if (gameData == null) return;
        gameData.introPlayed = true;
        SaveGame();
    }

    public Dictionary<string, GameData> GetAllProfilesGameData() => dataHandler.LoadAllProfiles();

    public void SaveCheckpoint(Vector3 pos)
    {
        gameData.lastScene = SceneManager.GetActiveScene().name;
        gameData.playerPosition = pos;
        gameData.checkpointScene = gameData.lastScene;
        gameData.checkpointPosition = pos;
        gameData.checkpointHealth = StatsManager.Instance != null ? StatsManager.Instance.maxHealth : gameData.maxHealth;
        SaveGame();
    }

    public string GetLastSavedScene()
        => gameData != null && !string.IsNullOrEmpty(gameData.lastScene) ? gameData.lastScene : SceneManager.GetActiveScene().name;

    public string GetCheckpointScene()
        => gameData != null && !string.IsNullOrEmpty(gameData.checkpointScene) ? gameData.checkpointScene : GetLastSavedScene();

    public bool HasCheckpoint()
        => gameData != null && !string.IsNullOrEmpty(gameData.checkpointScene);

    private void HardResetAllObjects()
    {
        if (WorldTime.Instance != null) WorldTime.Instance.ResetToMorning();
        var blank = new GameData("", "");
        foreach (var obj in FindAllDataPersistenceObjects())
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.LoadData(blank);
        }
    }

    private void ApplyDataToAll(GameData data)
    {
        if (dataPersistenceObjects == null) return;
        foreach (var obj in dataPersistenceObjects)
        {
            if (obj is MonoBehaviour mb && mb == null) continue;
            obj.LoadData(data);
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
        => FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>().ToList();
}
