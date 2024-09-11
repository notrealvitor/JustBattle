using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;   // Name of the enemy
    public Sprite enemySprite; // Sprite of the enemy
    public int healthMax;      // Maximum health of the enemy
    public int damage;         // Damage value of the enemy
}