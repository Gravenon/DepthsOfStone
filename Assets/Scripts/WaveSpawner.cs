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
}

public class WaveSpawner : MonoBehaviour, IDataPersistence
{
    [SerializeField] private SceneChanger sceneChanger;

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    public WaveData[] waves;

    public GameObject easyEnimiesPrefab;
    public GameObject hardEnimiesPrefab;
    public GameObject distantEnimiesPrefab;

    private int nextWave = 0;

    private bool waveComplete = false;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    private bool transitionTriggered = false;

    public SpawnState State => state;
    public float WaveCountdown => waveCountdown;
    public int NextWave => nextWave + 1;

    private MusicManager musicManager;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        musicManager = FindFirstObjectByType<MusicManager>();
        musicManager.PlayCombatMusic();
        
        // Подписываемся на событие смерти игрока
        PlayerHealth.Died += OnPlayerDied;
    }

    void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        PlayerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (transitionTriggered) return;
        
        transitionTriggered = true;
        
        // Сбрасываем индекс волн
        nextWave = 0;
        
        // Сохраняем игру с обнулённым индексом
        DataPersistenceeManager.instance.SaveGame();
        DataPersistenceeManager.SuppressNextSave = true;
        
        // Переходим в другую сцену
        sceneChanger.ChangeScene();
    }

    void Update()
    {
        if (transitionTriggered) return;

        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            if (!waveComplete)
                waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");
        waveCountdown = timeBetweenWaves;
        waveComplete = true;
        state = SpawnState.COUNTING;

        nextWave++;
        
        // Проверяем, остались ли еще волны
        if (nextWave >= waves.Length)
        {
            // Все волны пройдены - переходим в другую сцену
            Debug.Log("All waves completed!");
            
            // Save now; suppress the automatic save that fires on scene unload
            // to avoid touching already-destroyed objects.
            DataPersistenceeManager.instance.SaveGame();
            DataPersistenceeManager.SuppressNextSave = true;
            
            transitionTriggered = true;
            sceneChanger.ChangeScene();
            musicManager.PlayCalmMusic();
        }
        else
        {
            // Еще есть волны - продолжаем
            Debug.Log("Preparing next wave...");
            waveComplete = false;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(WaveData _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.easyEnimies; i++)
        {
            SpawnEnemy(easyEnimiesPrefab);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        for (int i = 0; i < _wave.hardEnimies; i++)
        {
            SpawnEnemy(hardEnimiesPrefab);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        for (int i = 0; i < _wave.distantEnimies; i++)
        {
            SpawnEnemy(distantEnimiesPrefab);
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        state = SpawnState.WAITING;
        yield break;
    }

    void SpawnEnemy(GameObject prefab)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(prefab, sp.position, sp.rotation);
    }

    // IDataPersistence
    public void LoadData(GameData data)
    {
        nextWave = data.arenaWaveIndex;
    }

    public void SaveData(ref GameData data)
    {
        data.arenaWaveIndex = nextWave;
    }
}
