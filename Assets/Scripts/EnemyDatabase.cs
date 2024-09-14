using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Enemies/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyData[] storedEnemies; // Array to store enemies

    // Function to store enemies from a CSV sheet into the array
    public void StoreEnemiesFromSheet(string sheetName)
    {
        // Load enemy data from the specified CSV file using DataSheetsHandling
        List<EnemyData> loadedEnemies = DataSheetsHandling.LoadEnemyDataFromCSV(sheetName);

        // Store the enemies in the array
        storedEnemies = loadedEnemies.ToArray();
    }

    // Function to get a random enemy from the stored array
    public EnemyData GetRandomStoredEnemy()
    {
        if (storedEnemies == null || storedEnemies.Length == 0)
        {
            Debug.LogError("No enemies are stored in the database!");
            return null;
        }

        int randomIndex = Random.Range(0, storedEnemies.Length);
        return storedEnemies[randomIndex];
    }
}