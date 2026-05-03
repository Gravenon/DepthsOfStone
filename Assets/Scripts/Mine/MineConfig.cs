using UnityEngine;

[CreateAssetMenu(fileName = "MineConfig")]
public class MineConfig : ScriptableObject
{
    public string MineName;
    [TextArea] public string itemDescription;
    public string sceneName;

    [System.Serializable]
    public class OreSpawnData
    {
        public GameObject orePrefab;
        public int minAmount;
        public int maxAmount;
    }

    public GameObject[] enemyPrefabs;
    public int enemiesCount = 3;
    public OreSpawnData[] ores;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public LayerMask obstacleLayer;
}
