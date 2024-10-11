using UnityEngine;
using System.IO;
using Unity.VisualScripting.FullSerializer;

public class SaveSystem
{

    private static string savePath = Application.persistentDataPath + "/savegame.json";

    // Save player data to a JSON file
    public static void SaveToFile(PlayerData saveData)
    {
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }

    // Load player data from the JSON file
    public static PlayerData LoadFromFile()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<PlayerData>(json);
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

    public static bool SaveFileExists()
    {
        return File.Exists(savePath);
    }
}
