using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

public static class DataSheetsHandling
{
    // Function to load enemy data from a CSV and return a list of CharacterData
    public static List<CharacterData> LoadEnemyDataFromCSV(string fileName)
    {
        List<CharacterData> enemyList = new List<CharacterData>();

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
                string spriteFileName = data[1].Trim(); // Enemy sprite image
                string baseStat = data[2]; // Base stat: STR, DEX, INT
                int abilityLevel = int.Parse(data[3]); // Ability level
                string description = data[4]; // Enemy description

                // Parse abilities from the 5th column (data[5]), separated by semicolons
                string[] abilities = data[5].Split(';');

                // Parse the baseType from column 6
                string baseType = data[6];

                // Load the sprite image
                Sprite enemySprite = LoadSprite(spriteFileName + ".png");

                // Create a new CharacterData ScriptableObject
                CharacterData characterData= ScriptableObject.CreateInstance<CharacterData>();
                characterData.charName = name;
                characterData.Sprite = enemySprite;
                characterData.baseStat = baseStat;
                characterData.abilityLevel = abilityLevel;
                characterData.description = description;
                characterData.abilities = abilities;  // Assign the parsed abilities
                characterData.baseType = baseType;    // Assign baseType

                // Add the new enemy to the list
                enemyList.Add(characterData);
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