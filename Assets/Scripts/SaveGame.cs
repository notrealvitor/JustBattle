using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";

    // Save player data to a JSON file
    public static void SaveToFile(PlayerSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }

    // Load player data from the JSON file
    public static PlayerSaveData LoadFromFile()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerSaveData>(json);
        }
        else
        {
            return null; // No save file exists
        }
    }

    // Delete save data (e.g., if the player dies)
    public static void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}