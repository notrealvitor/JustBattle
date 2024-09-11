using UnityEngine;
using TMPro;

public class CharacterStatus : MonoBehaviour
{
    [Header("Character Stats")]
    public string characterName;        // Name of the character
    [SerializeField] private int health; // Current health (private, not editable in Inspector)
    public int healthMax;               // Max health (editable in Inspector)
    public int damage;                  // Character's base damage

    [Header("PlayerSettings")]
    public bool isPlayer = false;
    public Sprite PlayerSpriteImage;

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
            HUD_CharacterName = characterHUDPrefab.transform.Find("HUD_CharName").GetComponent<TMP_Text>();
            HUD_CharacterHealth = characterHUDPrefab.transform.Find("HUD_CharHealth").GetComponent<TMP_Text>();
            characterHUDPrefab.SetActive(false);
        }

        if (isPlayer)
        {
            SetupCharacter("Player", PlayerSpriteImage, 100, 10); //HERE WE COULD ADD DIFERENT PLAYERS to Start
        }

        // Update the initial HUD state
        UpdateHUD();
    }

    // Setup function to initialize character status
    public void SetupCharacter(string name, Sprite image, int maxHealth, int dmg)
    {
        if (characterHUDPrefab != null)
        {
            characterSprite = transform.Find("CharacterImage").GetComponent<SpriteRenderer>();
        }

        characterName = name;
        healthMax = maxHealth;
        damage = dmg;
        health = healthMax; // Reset health to max


        // Set the sprite in the SpriteRenderer found in the child object
        if (characterSprite != null)
        {
            characterSprite.sprite = image;
        }

        // Update the HUD values accordingly
        UpdateHUD();
    }



    // Updates HUD elements like name and health display
    public void UpdateHUD()
    {
        if (HUD_CharacterName != null)
        {
            HUD_CharacterName.text = characterName;
        }

        if (HUD_CharacterHealth != null)
        {
            HUD_CharacterHealth.text = $"{health} / {healthMax}";
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

    // Checks if the character is dead
    public bool IsDead()
    {
        return health <= 0;
    }
}