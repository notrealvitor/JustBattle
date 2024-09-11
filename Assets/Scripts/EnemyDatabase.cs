using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Enemies/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> enemies; // List of all available enemies

    // Function to get a random enemy
    public EnemyData GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemies.Count);
        return enemies[randomIndex];
    }
}