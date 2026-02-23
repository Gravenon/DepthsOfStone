using UnityEngine;

public class MineGeneration : MonoBehaviour
{
    public MineConfig mineConfig;

    [SerializeField]
    private int nightStartHour = 20;

    [SerializeField]
    private int nightEndHour = 6;

    [SerializeField]
    private float nightEnemyMultiplier = 2f;

    [SerializeField]
    private Vector2Int nightExtraEnemiesRange = new Vector2Int(2, 5);

    [SerializeField]
    private float nightOreMultiplier = 1.5f;

    [SerializeField]
    private Vector2Int nightExtraOreRange = new Vector2Int(1, 3);

    private void Start()
    {
        SpawnOre();
        SpawnEnemies();
    }

    private void SpawnOre()
    {
        bool isNightTime = IsNightTime();

        foreach (var oreData in mineConfig.ores)
        {
            int count = Random.Range(oreData.minAmount, oreData.maxAmount + 1);

            if (isNightTime)
            {
                count = Mathf.CeilToInt(count * Mathf.Max(1f, nightOreMultiplier));
                count += Random.Range(nightExtraOreRange.x, nightExtraOreRange.y + 1);
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Random.Range(mineConfig.spawnAreaMin.x, mineConfig.spawnAreaMax.x),
                    Random.Range(mineConfig.spawnAreaMin.y, mineConfig.spawnAreaMax.y));

                if (pos != Vector2.zero)
                {
                    GameObject oreObject = Instantiate(oreData.orePrefab, pos, Quaternion.identity);
                }
            }
        }
    }

    public void SpawnEnemies()
    {
        int enemyCount = mineConfig.enemiesCount + Random.Range(0, 3); // Add 0-2 extra enemies for variability

        if (IsNightTime())
        {
            enemyCount = Mathf.CeilToInt(enemyCount * Mathf.Max(1f, nightEnemyMultiplier));
            enemyCount += Random.Range(nightExtraEnemiesRange.x, nightExtraEnemiesRange.y + 1);
        }

        for(int i = 0; i < enemyCount; i++)
        {
            Vector2 pos = new Vector2(
                    Random.Range(mineConfig.spawnAreaMin.x, mineConfig.spawnAreaMax.x),
                    Random.Range(mineConfig.spawnAreaMin.y, mineConfig.spawnAreaMax.y));

            GameObject enemyPrefab = mineConfig.enemyPrefabs[Random.Range(0, mineConfig.enemyPrefabs.Length)];
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }

    private bool IsNightTime()
    {
        WorldTime worldTime = WorldTime.Instance;

        if (worldTime == null)
        {
            worldTime = FindFirstObjectByType<WorldTime>();
        }

        if (worldTime == null)
        {
            return false;
        }

        int hour = worldTime.CurrentTime.Hours;

        if (nightStartHour > nightEndHour)
        {
            return hour >= nightStartHour || hour < nightEndHour;
        }

        return hour >= nightStartHour && hour < nightEndHour;
    }
}
