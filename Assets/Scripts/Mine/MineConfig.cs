using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MineConfig")]
public class MineConfig : ScriptableObject
{
    [System.Serializable]
    public class OreSpawnData
    {
        public GameObject orePrefab;
        //public ItemSO itemSO;
        public int minAmount;
        public int maxAmount;
    }

    public OreSpawnData[] ores;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public LayerMask obstacleLayer;
}
