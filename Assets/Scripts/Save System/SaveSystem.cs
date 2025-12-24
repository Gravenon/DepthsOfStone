using UnityEngine;
using System.IO;

public static class SaveSystem
{ 
    private static string path = Application.persistentDataPath + "/player.json";
    public static void SavePlayer(StatsManager stats, Transform playerTransform)
    {
        PlayerData data = new PlayerData(stats, playerTransform);

        string json = JsonUtility.ToJson(data);
        string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        File.WriteAllText(path, encoded);

        Debug.Log("Game Saved to " + path);
    }

    public static PlayerData LodadPlayer()
    {
        if (File.Exists(path))
        {
            string encoded = File.ReadAllText(path);
            string json = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encoded));

            PlayerData data = JsonUtility.FromJson<PlayerData>(json);


            Debug.Log("Game Loaded from " + path);
            return data;

        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }

}
