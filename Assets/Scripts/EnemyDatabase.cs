using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Enemies/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public CharacterData[] storedEnemies; // Array to store enemies

    // Function to store enemies from a CSV sheet into the array
    public void StoreEnemiesFromSheet(string sheetName)
    {
        // Load enemy data from the specified CSV file using DataSheetsHandling
        List<CharacterData> loadedEnemies = DataSheetsHandling.LoadEnemyDataFromCSV(sheetName);

        // Store the enemies in the array
        storedEnemies = loadedEnemies.ToArray();
    }

    // Function to get a random enemy from the stored array
    public CharacterData GetRandomStoredEnemy()
    {
        if (storedEnemies == null || storedEnemies.Length == 0)
        {
            Debug.LogError("No enemies are stored in the database!");
            return null;
        }

        int randomIndex = Random.Range(0, storedEnemies.Length);

        return storedEnemies[randomIndex];
    }

    // Function to get a random enemy with abilityLevel <= specified level
    public CharacterData GetRandomEnemyByLevel(int desiredAbilityLevel)
    {
        if (storedEnemies == null || storedEnemies.Length == 0)
        {
            Debug.LogError("No enemies are stored in the database!");
            return null;
        }

        // Filter enemies by abilityLevel == maxAbilityLevel
        List<CharacterData> filteredEnemies = new List<CharacterData>();
        foreach (var enemy in storedEnemies)
        {
            if (enemy.abilityLevel == desiredAbilityLevel)
            {
                filteredEnemies.Add(enemy);
            }
        }

        if (filteredEnemies.Count == 0)
        {
            Debug.LogError($"No enemies found with abilityLevel <= {desiredAbilityLevel}.");
            return null;
        }

        // Get a random enemy from the filtered list
        int randomIndex = Random.Range(0, filteredEnemies.Count);
        return filteredEnemies[randomIndex];
    }
}