using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataSheetsHandling
{
    // Function to load enemy data from a CSV and return a list of EnemyData
    public static List<EnemyData> LoadEnemyDataFromCSV(string fileName)
    {
        List<EnemyData> enemyList = new List<EnemyData>();

        // Construct the file path relative to the Resources/DataSheets folder
        string fullPath = "DataSheets/" + fileName;

        // Load the CSV file from the Resources folder
        TextAsset csvFile = Resources.Load<TextAsset>(fullPath);

        if (csvFile == null)
        {
            Debug.LogError("CSV file not found at path: " + fullPath);
            return enemyList;
        }

        // Read all lines in the CSV file
        string[] dataLines = csvFile.text.Split('\n');

        // Skip the header line (index 0)
        for (int i = 1; i < dataLines.Length; i++)
        {
            string line = dataLines[i];
            if (!string.IsNullOrEmpty(line))
            {
                string[] data = line.Split(',');

                // Parse enemy data
                string name = data[0];
                int healthMax = int.Parse(data[1]);
                int damage = int.Parse(data[2]);
                string spriteFileName = data[3].Trim();  // Trim any whitespace

                // Automatically add ".png" to the sprite filename
                spriteFileName = spriteFileName + ".png";

                // Load the sprite from the Assets/Art/Sprites folder
                Sprite enemySprite = LoadSprite(spriteFileName);

                // Create a new EnemyData ScriptableObject
                EnemyData enemyData = ScriptableObject.CreateInstance<EnemyData>();
                enemyData.enemyName = name;
                enemyData.healthMax = healthMax;
                enemyData.damage = damage;
                enemyData.enemySprite = enemySprite;

                // Add the new enemy to the list
                enemyList.Add(enemyData);
            }
        }

        return enemyList;
    }

    // Helper function to load a sprite from the Resources folder
    private static Sprite LoadSprite(string fileName)
    {
        // Load the sprite from the Assets/Art/Sprites folder using Resources
        string spritePath = "Art/Sprites/" + Path.GetFileNameWithoutExtension(fileName);
        return Resources.Load<Sprite>(spritePath);
    }
}