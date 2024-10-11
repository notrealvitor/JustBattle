using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;
    public string baseStat;     
    public int abilityLevel;
    public string description; // Description of the enemy
    public string[] abilities; // Array of abilities
    public string baseType;     //this could be made into multiple types too to increase complexity of the game

    public int strenght;
    public int dextery;
    public int intelligence;

    public int level;
    public int healthMax;
    public int manaMax;
    public int damage;
    public int speed;
}