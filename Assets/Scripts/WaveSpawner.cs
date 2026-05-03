using System.Collections;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string name;
    public int easyEnimies;
    public int hardEnimies;
    public int distantEnimies;
    public float rate;
    public bool isBossWave;    // if true — spawns boss instead of regular enemies
}

public class WaveSpawner : MonoBehaviour, IDataPersistence
{
    [SerializeField] private SceneChanger sceneChanger;

    public enum SpawnState { SPAWNING, WAITING, COUNTING }

    public WaveData[] waves;

    public GameObject easyEnimiesPrefab;
    public GameObject hardEnimiesPrefab;
    public GameObject distantEnimiesPrefab;
    public GameObject bossPrefab;               // assign Boss prefab here

    private int nextWave = 0;
    private bool waveComplete = false;
    private bool transitionTriggered = false;

    public Transform[] spawnPoints;
    public Transform   bossSpawnPoint;          // centre of arena

    public float timeBetweenWaves = 4f;
    private float waveCountdown;
    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    public SpawnState State      => state;
    public float      WaveCountdown => waveCountdown;
    public int        NextWave   => nextWave + 1;

    private MusicManager musicManager;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        musicManager  = FindFirstObjectByType<MusicManager>();
        musicManager.PlayCombatMusic();
        PlayerHealth.Died += OnPlayerDied;
    }

    void OnDestroy() => PlayerHealth.Died -= OnPlayerDied;

    private void OnPlayerDied()
    {
        if (transitionTriggered) return;
        transitionTriggered = true;
        nextWave = 0;
        DataPersistenceeManager.instance.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        sceneChanger.ChangeScene();
    }

    void Update()
    {
        if (transitionTriggered) return;

        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive()) WaveCompleted();
            return;
        }

        if (waveCountdown <= 0f)
        {
            if (state != SpawnState.SPAWNING)
                StartCoroutine(SpawnWave(waves[nextWave]));
        }
        else
        {
            if (!waveComplete)
                waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        waveCountdown = timeBetweenWaves;
        waveComplete  = true;
        state         = SpawnState.COUNTING;
        nextWave++;

        if (nextWave >= waves.Length)
        {
            DataPersistenceeManager.instance.SaveGame();
            DataPersistenceeManager.SuppressNextSave = true;
            transitionTriggered = true;
            musicManager.PlayCalmMusic();
            sceneChanger.ChangeScene();
        }
        else
        {
            waveComplete = false;
        }
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        state = SpawnState.SPAWNING;

        if (wave.isBossWave)
        {
            // Boss wave — spawn boss at centre spawn point, no regular enemies
            if (bossPrefab != null)
            {
                Transform sp = bossSpawnPoint != null ? bossSpawnPoint : spawnPoints[0];
                Instantiate(bossPrefab, sp.position, sp.rotation);
            }
        }
        else
        {
            for (int i = 0; i < wave.easyEnimies; i++)
            {
                SpawnEnemy(easyEnimiesPrefab);
                yield return new WaitForSeconds(1f / wave.rate);
            }
            for (int i = 0; i < wave.hardEnimies; i++)
            {
                SpawnEnemy(hardEnimiesPrefab);
                yield return new WaitForSeconds(1f / wave.rate);
            }
            for (int i = 0; i < wave.distantEnimies; i++)
            {
                SpawnEnemy(distantEnimiesPrefab);
                yield return new WaitForSeconds(1f / wave.rate);
            }
        }

        state = SpawnState.WAITING;
    }

    void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return;
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, sp.position, sp.rotation);
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            return GameObject.FindGameObjectWithTag("Enemy") != null;
        }
        return true;
    }

    public void LoadData(GameData data) => nextWave = data.arenaWaveIndex;
    public void SaveData(ref GameData data) => data.arenaWaveIndex = nextWave;
}

