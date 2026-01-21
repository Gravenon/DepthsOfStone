using UnityEngine;

[System.Serializable]
public class WorldData
{
    public string sceneName;
    public int arenaWaveIndex;

    public WorldData(string sceneName, int arenaWaveIndex)
    {
        this.sceneName = sceneName;
        this.arenaWaveIndex = arenaWaveIndex;
    }
}
