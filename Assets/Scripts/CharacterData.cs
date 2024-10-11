using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string charName;
    public Sprite Sprite;
    public string baseStat;     
    public int abilityLevel;
    public string description; // Description of the enemy
    public string[] abilities; // Array of abilities
    public string baseType;     //this could be made into multiple types too to increase complexity of the game

    public int strength;
    public int dexterity;
    public int intelligence;

    public int level;
    public int healthMax;
    public int health;
    public int manaMax;
    public int damage;
    public float speed;
    public int speedMax;
}