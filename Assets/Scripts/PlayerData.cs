using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int experience;
    public int gold;
    public int numberOfFights;
    public CharacterData playerCharacterData;

    public PlayerData(int experience, int gold, int NumberOfFights, CharacterData playerCharacterData)
    {
        this.experience = experience;
        this.gold = gold;
        this.numberOfFights = 0;
        this.playerCharacterData = playerCharacterData;
    }


    // Getter and Setter for Strength
    public int GetStrength() => playerCharacterData.strength;
    public void SetStrength(int value) => playerCharacterData.strength = value;

    // Getter and Setter for Dexterity
    public int GetDexterity() => playerCharacterData.dexterity;
    public void SetDexterity(int value) => playerCharacterData.dexterity = value;

    // Getter and Setter for Intelligence
    public int GetIntelligence() => playerCharacterData.intelligence;
    public void SetIntelligence(int value) => playerCharacterData.intelligence = value;

}
