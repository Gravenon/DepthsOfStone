using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class FilePathHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FilePathHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(string profilID)
    {
        string fullPath = Path.Combine(dataDirPath, profilID, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error loading game data: " + e.Message + " at path: " + fullPath);
                return null;
            }

        }
        return loadedData;
    }

    public Dictionary<string, GameData>LoadAllProfiles()
    {
        Dictionary<string, GameData> allProfilesData = new Dictionary<string, GameData>();
        
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach(DirectoryInfo dirInfo in dirInfos)
        {
            string profilID = dirInfo.Name;

            string fullPath = Path.Combine(dataDirPath, profilID, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("No save file found for profile: " + profilID + " at path: " + fullPath);
                continue;
            }

            GameData profileData = Load(profilID);

            if(profileData != null)
            {
                allProfilesData.Add(profilID, profileData);
            }
            else
            {
                Debug.LogWarning("Failed to load data for profile: " + profilID + " at path: " + fullPath);
            }
        }

        return allProfilesData;

    }

    public void Delete(string profilID)
    {
        string fullPath = Path.Combine(dataDirPath, profilID, dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                string dir = Path.GetDirectoryName(fullPath);
                if (Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0)
                    Directory.Delete(dir);
            }
            else
            {
                Debug.LogWarning("[FilePathHandler] No save file to delete at: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[FilePathHandler] Error deleting save: " + e.Message);
        }
    }

    public void Save(GameData gameData, string profilID)
    {
        string fullPath = Path.Combine(dataDirPath, profilID, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string json  = JsonUtility.ToJson(gameData, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving game data: " + e.Message + " at path: " + fullPath);
        }
    }
}
