using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Linq;

[CreateAssetMenu(fileName = "MineConfig")]
public class MineConfig : ScriptableObject
{
    [SerializeField] private string mineNameKey;
    [SerializeField] [TextArea]private string mineDescriptionKey;
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

    public string MineName =>
    LocalizationSettings.StringDatabase.GetLocalizedString(
        "Mines",
        mineNameKey
    );

    public string MineDescription =>
        LocalizationSettings.StringDatabase.GetLocalizedString(
            "Mines",
            mineDescriptionKey
        );

    public string GetOreList()
    {
        List<string> names = new();

        foreach (var ore in ores)
        {
            if (ore.orePrefab == null)
                continue;

            Ore oreComponent = ore.orePrefab.GetComponent<Ore>();

            if (oreComponent != null)
                names.Add(oreComponent.OreName);
        }

        return string.Join(", ", names.Distinct());
    }
}
