using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class WorldSaveSystem
{
    private static string path => Application.persistentDataPath + "/world.dat";

    public static void SaveWorld(string scenaName, int arenaWaveIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        WorldData data = new WorldData(scenaName, arenaWaveIndex);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log("World saved to: " + path);
    }

    public static WorldData LoadWorld()
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        WorldData data = formatter.Deserialize(stream) as WorldData;
        stream.Close();

        return data;
    }
}
