using UnityEngine;
using TMPro;
using UnityEditor.U2D.Animation;
using System.Security.Cryptography.X509Certificates;

public class CharacterStatus : MonoBehaviour
{
    [Header("Character Stats")]
    public string characterName;            // Name of the character
    [SerializeField] private int health;    // Current health (private, not editable in Inspector)
    public int healthMax;                   // Max health (editable in Inspector)
    public float speed;
    public int speedMax;                    //Agilty to calculate turn speed
    public int damage;                      // Character's base damage
    public int experience;
    public string[] skillNames;

    public CharacterData charCharacterData;

    public SkillComponent charSkillComponent;

    [Header("PlayerSettings")]
    public bool isPlayer = false;

    [Header("HUD References")]
    public GameObject characterHUDPrefab; // Reference to the CharacterHUD prefab
    private TMP_Text HUD_CharacterName;    // Reference to the name text in HUD
    private TMP_Text HUD_CharacterHealth;  // Reference to the health text in HUD
    private SpriteRenderer characterSprite; // Automatically get the SpriteRenderer from child
    

    void Start()
    {
        // Automatically get the SpriteRenderer from the child called "CharacterImage"
        characterSprite = transform.Find("CharacterImage").GetComponent<SpriteRenderer>();

        // Initialize health to max at the start
        health = healthMax;

        // Set up references to HUD components (if prefab is assigned)
        if (characterHUDPrefab != null)
        {
            //print("chracterHUD started");
            HUD_CharacterName = characterHUDPrefab.transform.Find("HUD_CharName").GetComponent<TMP_Text>();
            HUD_CharacterHealth = characterHUDPrefab.transform.Find("HUD_CharHealth").GetComponent<TMP_Text>();            
        }
        else print("characterHUDPrefab is invalid in " + gameObject.name);

        charSkillComponent = this.GetComponent<SkillComponent>();

        // Update the initial HUD state
        //UpdateHUD();
    }

    // Setup function to initialize character status
    public void SetupCharacter(string name, Sprite image, int maxHealth, int dmg)
    {
        if (characterHUDPrefab != null)
        {
            characterSprite = transform.Find("CharacterImage").GetComponent<SpriteRenderer>();
            if (characterSprite != null) characterSprite.sprite = image; // Set the sprite in the SpriteRenderer found in the child object
        }

        characterName = name;
        healthMax = maxHealth;
        damage = dmg;
        health = healthMax; // Reset health to max

        // Update the HUD values accordingly
        UpdateHUD();
    }

    public void SetupCharacter(CharacterData characterData = null, int DiceRolls = 4, int DiceSize = 6) // need to remember it is 4 rolls but one is discarted
    {
        if (characterHUDPrefab != null)
        {
            characterSprite = transform.Find("CharacterImage").GetComponent<SpriteRenderer>();
            if (characterSprite != null) characterSprite.sprite = characterData.Sprite; // Set the sprite in the SpriteRenderer found in the child object
        }

        if (characterData != null)        
        {            
            // Roll dice for attributes
            int rolledStrength = GameUtilities.RollDice(DiceRolls, DiceSize, 1);       // Roll dice for strength and discard the 1 of the lowest value
            int rolledDexterity = GameUtilities.RollDice(DiceRolls, DiceSize, 1);      // Roll dice for dexterity and discard the 1 of the lowest value
            int rolledIntelligence = GameUtilities.RollDice(DiceRolls, DiceSize, 1);    // Roll dice for intelligence and discard the 1 of the lowest value

            // Set attributes based on rolls
            characterData.strength = rolledStrength;
            characterData.dexterity = rolledDexterity;
            characterData.intelligence = rolledIntelligence;

            // Find the highest rolled attribute
            int highestAttribute = Mathf.Max(rolledStrength, rolledDexterity, rolledIntelligence);

            // Swap base stat with highest attribute if needed
            switch (characterData.baseStat)
            {
                case "STR":
                    if (highestAttribute != rolledStrength) SwapAttributes(ref characterData.strength, ref highestAttribute); // Swap highest attribute with strength
                    break;
                case "DEX":
                    if (highestAttribute != rolledDexterity) SwapAttributes(ref characterData.dexterity, ref highestAttribute); // Swap highest attribute with dexterity
                    break;
                case "INT":
                    if (highestAttribute != rolledIntelligence) SwapAttributes(ref characterData.intelligence, ref highestAttribute); // Swap highest attribute with intelligence
                    break;
            }

            // Calculate stats
            CalculateCharLevel(characterData); // we need to calculate charLevel first otherwise everything breaks if it is 0, still thinking the best way to do that based on the playerLevel and the rooms beaten
            characterName = characterData.charName;

            // Health Calculation
            healthMax = CalculateStat(characterData.strength, characterData.level, 2.0f);  // Example multiplier for health
            health = healthMax;

            // Damage Calculation
            switch (characterData.baseStat)
            {
                case "DEX":
                    damage = CalculateStat(characterData.dexterity, characterData.level, 1.5f); 
                    break;
                case "STR":
                    damage = CalculateStat(characterData.strength, characterData.level, 1.5f);
                    break;
                case "INT":
                    damage = CalculateStat(characterData.intelligence, characterData.level, 1.5f); 
                    break;
            }

            // Speed Calculation
            speedMax = CalculateSpeed(characterData.dexterity);
            //speedMax = CalculateStat(characterData.dexterity, characterData.level, 1.5f);
            speed = speedMax;

            print(gameObject.name + "stats is: STR " + characterData.strength + " DEX " + characterData.dexterity + " INT " + characterData.intelligence);
            print(gameObject.name + " healthMAX: " + healthMax + " DMG: " + damage + " SPD " + speed);
            charCharacterData = characterData;
        }
        else
        {
            print("CharacterData not found in the CharacterStatus!");
        }
    }

    public void LoadCharacter()
    {
        LoadCharacterData();

        if (characterHUDPrefab != null)
        {
            characterSprite = transform.Find("CharacterImage").GetComponent<SpriteRenderer>();
            if (characterSprite != null) characterSprite.sprite = charCharacterData.Sprite; // Set the sprite in the SpriteRenderer found in the child object
        }        
    }

    // Helper function to swap two attributes
    private void SwapAttributes(ref int baseStat, ref int highestStat)
    {
        int temp = baseStat;
        baseStat = highestStat;
        highestStat = temp;
    }

    // This function calculates a stat (e.g., Health, Damage, Speed) based on input values.
    private int CalculateStat(int stat, int level, float multiplier)
    {
        
        return Mathf.RoundToInt(stat * multiplier + level * multiplier);
        
    }

    private int CalculateSpeed(int stat)
    {
        // Ensure minimum speed is 3
        int baseSpeed = 3;

        // Calculate the additional speed based on dexterity (1 point every 5 dexterity)
        int extraSpeed = stat / 5;  // Every 5 dex adds 1 to speed

        // Final speed calculation
        return baseSpeed + extraSpeed;
    }

    private int CalculateCharLevel(CharacterData characterData = null)
    {
        if (characterData != null)
            return characterData.level = 1;
        else
            return characterData.level = 1;  // Adjust for player calculation
    }

    // Updates HUD elements like name and health display
    public void UpdateHUD() //simplify this after all the HUD is decided
    {

        if (HUD_CharacterName != null)
        {
            HUD_CharacterName.text = characterName;
        }
        else
        {
            //print("HUD_CharacterName is invalid in " + gameObject.name);
            HUD_CharacterName = characterHUDPrefab.transform.Find("HUD_CharName").GetComponent<TMP_Text>();
            HUD_CharacterName.text = characterName;
        }

        if (HUD_CharacterHealth != null)
        {
            HUD_CharacterHealth.text = $"H: {health} / {healthMax}"; 
        }
        else
        {
            //print("HUD_CharacterHealth is invalid in " + gameObject.name);
            HUD_CharacterHealth = characterHUDPrefab.transform.Find("HUD_CharHealth").GetComponent<TMP_Text>();
            HUD_CharacterHealth.text = $"H: {health} / {healthMax}";
        }
    }

    // Function to handle taking damage
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        print(characterName + " took damage! Health is now " + health);
        if (health < 0) health = 0;

        // Update the HUD after damage is applied
        UpdateHUD();
    }


    public void LoadCharacterData()
    {
        health = charCharacterData.health;
        healthMax = charCharacterData.healthMax;
        speed = charCharacterData.speed;
        speedMax = charCharacterData.speedMax;
        characterName = charCharacterData.charName;
        damage = charCharacterData.damage;
    }

    public void SaveToCharacterData()
    {
        charCharacterData.health = GetHealth();
        charCharacterData.healthMax = healthMax;
        charCharacterData.speed = speed;
        charCharacterData.speedMax = speedMax;
        charCharacterData.charName = characterName;
        charCharacterData.damage = damage;
    }

    // Setter function for health
    public void SetHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, healthMax); // Ensure health is within valid range
        UpdateHUD(); // Update UI after setting health
    }

    // Getter function for health
    public int GetHealth()
    {
        return health;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = Mathf.Clamp(newSpeed, 0, speedMax); // Ensure health is within valid range
        UpdateHUD(); // Update UI after setting speed
    }

    // Getter function for health
    public float GetSpeed()
    {
        return speed;
    }

    // Checks if the character is dead
    public bool IsDead()
    {
        return health <= 0;
    }
}