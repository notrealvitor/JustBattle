using UnityEngine;
using TMPro;
using UnityEngine.UI; // For UI components like Button
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance; // Singleton instance

    private GameObject player; // Player object
    private GameObject enemy;  // Enemy object

    private GameObject playerHUD;  // Reference to the player's HUD
    private GameObject enemyHUD;   // Reference to the enemy's HUD

    private TMP_Text dialogueText; // Dialogue box for text
    private GameObject ActionButtons; // UI for battle buttons (Attack, Skills, etc.)
    private Button attackButton; // Attack Button reference

    public EnemyDatabase enemyDatabase; // Public field to manually assign in Inspector

    private CharacterStatus playerStatus;
    private CharacterStatus enemyStatus;


    private bool battleStarted = false;
    private bool battleEnded = false;  // Flag to track if the battle has ended

    void Awake()
    {
        // Implementing Singleton pattern to ensure one instance of CombatManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Ensure the CombatManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate CombatManager instances
        }
    }

    void Start()
    {
        SetupBattle();
        //StartCoroutine(DelayedSetupBattle(0.5f)); // Adding a delay of 0.5 seconds for setup
    }

    void FindReferences()
    {
        // Dynamically assign the GameObjects by name and check for nulls
        player = GameObject.Find("PlayerPrefab");
        if (player == null) Debug.LogError("PlayerPrefab not found!");

        enemy = GameObject.Find("EnemyPrefab");
        if (enemy == null) Debug.LogError("EnemyPrefab not found!");

        dialogueText = GameObject.Find("TextNotice")?.GetComponent<TMP_Text>();
        if (dialogueText == null) Debug.LogError("TextNotice not found or TMP_Text component is missing!");

        ActionButtons = GameObject.Find("HUD_Buttons");
        if (ActionButtons == null) Debug.LogError("HUD_Buttons not found!");

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(false);
            // Find the Attack Button as a child of ActionButtons using the correct name "AttackBTN"
            attackButton = ActionButtons.transform.Find("AttackBTN")?.GetComponent<Button>();
            if (attackButton == null)
            {
                Debug.LogError("AttackBTN not found or missing Button component!");
            }
            else
            {
                attackButton.onClick.AddListener(PlayerAttack); // Dynamically add the PlayerAttack function
            }
        }

        // Get the CharacterStatus from the player and enemy GameObjects
        if (player != null)
        {
            playerStatus = player.GetComponent<CharacterStatus>();
            if (playerStatus == null) Debug.LogError("CharacterStatus component not found on PlayerPrefab!");
        }

        if (enemy != null)
        {
            enemyStatus = enemy.GetComponent<CharacterStatus>();
            if (enemyStatus == null) Debug.LogError("CharacterStatus component not found on EnemyPrefab!");
        }

        // Find PlayerHUD and EnemyHUD as UI elements
        playerHUD = GameObject.Find("PlayerHUD");
        enemyHUD = GameObject.Find("EnemyHUD");

        // If they are part of a Canvas or not found via GameObject.Find, try FindObjectsOfType for UI elements
        if (playerHUD == null)
        {
            playerHUD = GameObject.FindObjectOfType<Canvas>().transform.Find("PlayerHUD")?.gameObject;
        }
        if (enemyHUD == null)
        {
            enemyHUD = GameObject.FindObjectOfType<Canvas>().transform.Find("EnemyHUD")?.gameObject;
        }

        // Check for nulls to avoid issues
        if (playerHUD == null) Debug.LogError("Player CharacterHUD not found!");
        if (enemyHUD == null) Debug.LogError("Enemy CharacterHUD not found!");
    }

    IEnumerator DelayedSetupBattle(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay time
        SetupBattle();
    }

    void SetupBattle()
    {
        // Find the references after the scene is loaded
        FindReferences();

        // Ensure battle flags are set correctly
        battleStarted = false;
        battleEnded = false;

        // Get a random enemy from the enemy database
        EnemyData randomEnemy = enemyDatabase.GetRandomEnemy();

        // Set up enemy's stats and image using CharacterStatus
        enemyStatus.SetupCharacter(randomEnemy.enemyName, randomEnemy.enemySprite, randomEnemy.healthMax, randomEnemy.damage);

        enemy.SetActive(true); // Activate the enemy in the scene

        if (dialogueText != null)
        {
            dialogueText.text = $"A {randomEnemy.enemyName} appeared!";
        }

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(false); // Hide action buttons until battle starts
        }
    }

    void Update()
    {
        // Wait for any input to start the battle
        if (!battleStarted && !battleEnded && Input.anyKeyDown)
        {
            StartBattle();
        }

        // If the battle has ended and any key is pressed, load the next battle
        if (battleEnded && Input.anyKeyDown)
        {
            StartCoroutine(ReloadBattleScene());
        }
    }

    void StartBattle()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "Choose your action.";
        }

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(true); // Show battle UI (buttons)
        }

        // Activate the player's and enemy's HUDs
        if (playerHUD != null)
        {
            playerHUD.SetActive(true);
        }

        if (enemyHUD != null)
        {
            enemyHUD.SetActive(true);
        }

        battleStarted = true;
    }
    public void PlayerAttack()
    {
        // Don't allow actions if the battle has ended
        if (battleEnded) return;

        // Player attacks the enemy, dealing damage
        enemyStatus.TakeDamage(playerStatus.damage);

        if (dialogueText != null)
        {
            dialogueText.text = $"You attacked {enemyStatus.characterName} for {playerStatus.damage} damage!";
        }

        // Check if the enemy is dead
        if (enemyStatus.IsDead())
        {
            EndBattle(true); // Player wins
        }
        else
        {
            // Wait before switching to enemy turn
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        // Don't proceed with the enemy turn if the battle has ended
        if (battleEnded) yield break;

        dialogueText.text = $"{enemyStatus.characterName} turn!";

        ActionButtons.SetActive(false);

        yield return new WaitForSeconds(1); // Wait a second before enemy attacks

        if (dialogueText != null)
        {
            dialogueText.text = $"{enemyStatus.characterName} attacks!";
        }

        playerStatus.TakeDamage(enemyStatus.damage);

        if (dialogueText != null)
        {
            dialogueText.text = $"{enemyStatus.characterName} dealt {enemyStatus.damage} damage!";
        }

        // Check if the player is dead
        if (playerStatus.IsDead())
        {
            EndBattle(false); // Player loses
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartPlayerTurn(); // Start player turn again
        }
    }

    void StartPlayerTurn()
    {
        if (dialogueText != null)
        {
            dialogueText.text = "Your turn!";
        }

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(true); // Re-enable player action buttons
        }
    }

    void EndBattle(bool playerWon)
    {
        // Set battleEnded flag to prevent further actions
        battleEnded = true;

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(false);
        }

        if (playerWon)
        {
            StartCoroutine(LoadSceneAndShowText("WinScreen", $"You defeated {enemyStatus.characterName}!"));
        }
        else
        {
            StartCoroutine(LoadSceneAndShowText("LoseScreen", "You were defeated!"));
        }
    }

    IEnumerator LoadSceneAndShowText(string sceneName, string message)
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Once the scene is loaded, update the dialogue text
        TMP_Text newDialogueText = GameObject.Find("TextNotice").GetComponent<TMP_Text>(); // Ensure this is the correct name

        if (newDialogueText != null)
        {
            newDialogueText.text = message;
        }
    }

    IEnumerator ReloadBattleScene()
    {
        // Reload the BattleScene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene"); // Replace "BattleScene" with the actual scene name

        // Wait for the scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // After the scene is loaded, set up the battle again
        StartCoroutine(DelayedSetupBattle(0.5f)); // Optional delay for setting up the next battle
    }
}