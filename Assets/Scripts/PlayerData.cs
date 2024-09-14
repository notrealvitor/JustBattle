[System.Serializable]
public class PlayerSaveData
{
    public int health;
    public int maxHealth;
    public int experience;
    public int gold;

    // Add other attributes like mana, cooldown, etc. later as needed
    public PlayerSaveData(int health, int maxHealth, int experience, int gold)
    {
        this.health = health;
        this.maxHealth = maxHealth;
        this.experience = experience;
        this.gold = gold;
    }
}