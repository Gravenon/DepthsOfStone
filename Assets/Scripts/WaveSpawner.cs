using System.Collections;
using UnityEngine;



[System.Serializable]
public class WaveData
{
    public string name;
    public int easyEnimies;
    public int hardEnimies;
    public float rate;
}

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private SceneChanger sceneChanger;

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    public WaveData[] waves;

    public GameObject easyEnimiesPrefab;
    public GameObject hardEnimiesPrefab;

    private int nextWave = 0;

    private bool waveComplete = false;

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    private bool transitionTriggered = false;

    public SpawnState State
    {
        get { return state; }
    }

    public float WaveCountdown
    {
        get { return waveCountdown; }
    }

    public int NextWave
    {
        get { return nextWave + 1; }
    }
    void Start()
    {
        WorldData world = WorldSaveSystem.LoadWorld();
        
        if (world != null)
        {
            nextWave = world.arenaWaveIndex;
        }

        waveCountdown = timeBetweenWaves;
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
            if(!waveComplete)
            waveCountdown -= Time.deltaTime; 
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");
        waveCountdown = timeBetweenWaves;
        waveComplete = true;

        state = SpawnState.COUNTING;

        int nextWaveIndex = nextWave + 1;

        WorldSaveSystem.SaveWorld("Arena", nextWaveIndex);

        sceneChanger.ChangeScene();

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

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(GameObject prefab)
    {
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject _enemy = Instantiate(prefab, _sp.position, _sp.rotation);
    }

}
