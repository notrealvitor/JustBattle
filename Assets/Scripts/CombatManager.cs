using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using static GameManager;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Database")]
    public EnemyDatabase enemyDatabase;

    [HideInInspector] public CharacterStatus playerStatus;
    [HideInInspector] public CharacterStatus enemyStatus;

    public BattleState currentState;
    private GameManager gameManager;
    public TurnManager turnManager;

    [HideInInspector] public SkillComponent playerSkillComponent;
    [HideInInspector] public InventoryComponent playerInventoryComponent;

    private int levelThreshold = 0; // 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject gameManagerObject = GameObject.FindWithTag("GameManager");
        if (gameManagerObject != null) gameManager = gameManagerObject.GetComponent<GameManager>();
        enemyDatabase.StoreEnemiesFromSheet("EnemiesSheet1");

        SetupBattle();
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }

        //debug test for monsterLevelSorting
        // Example test: press the "space" key to simulate a battle won and get a random monster level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Simulate a battle won
            UpdateMonsterLevelProbabilities();

            gameManager.numberOfFights++;

            // Test the random monster level selection after probability adjustment
            int monsterLevel = GetRandomMonsterLevel();
            Debug.Log("Selected Monster Level: " + monsterLevel + "numberOfFights: " + gameManager.numberOfFights);

            // Print current probabilities for each level for debugging
            //Debug.Log("Current Level Probabilities:");
            foreach (var entry in levelProbabilities)
            {
                Debug.Log($"Level {entry.Key}: {entry.Value}%");
            }
        }
        //end of debug
    }

    void FindReferences()
    {
        playerStatus = GameObject.FindWithTag("Player")?.GetComponent<CharacterStatus>();
        enemyStatus = GameObject.FindWithTag("Enemy")?.GetComponent<CharacterStatus>();

        playerSkillComponent = playerStatus?.GetComponent<SkillComponent>();
        playerInventoryComponent = playerStatus?.GetComponent<InventoryComponent>();

        if (playerStatus == null || enemyStatus == null)
        {
            Debug.LogError("Player or Enemy CharacterStatus not found!");
        }

        UIManager.instance = FindObjectOfType<UIManager>();
    }

    void SetupBattle()
    {
        FindReferences();
        SetupEnemy();
        SetupPlayer();
        StartBattle();
    }

    void StartBattle()
    {
        BattleTextManager.Instance.DisplayMessageForSeconds($"A {enemyStatus.characterName} appeared!", 2.0f, () =>
        {
            // Assign UI elements through the UIManager
            UIManager.instance.SetupCombatUI(playerSkillComponent, playerInventoryComponent);

            //SetState(new PlayerTurnState(this));
            turnManager.StartTurnManager();
            turnManager.NextTurn();
        });

    }

    void SetupEnemy()
    {
        // Get the current number of fights from the GameManager
        int numberOfFights = gameManager.numberOfFights;

        // Dynamically calculate the enemy level based on the number of fights
        int enemyLevel = GetRandomMonsterLevel(); // Use the logic we previously discussed
        print( GetRandomMonsterLevel() + " was sorted for MonsterLevel" );

        // Get a random enemy of the calculated level
        CharacterData randomEnemy = enemyDatabase.GetRandomEnemyByLevel(enemyLevel); // Now this will be based on the dynamic enemyLevel

        if (randomEnemy == null)
        {
            Debug.LogError("No enemy data found!");
            return;
        }

        // Determine dice rolls and dice size based on numberOfFights
        int diceRolls = CalculateDiceRolls(gameManager.numberOfFights);
        int diceSize = CalculateDiceSize(gameManager.numberOfFights);

        // Initialize the enemy with the calculated rolls and dice size
        InitializeCharacter(enemyStatus, randomEnemy, 3, 6);

        enemyStatus.gameObject.SetActive(true);
        enemyStatus.UpdateHUD();
    }

    void SetupPlayer()
    {
        if (!SaveSystem.SaveFileExists())
        {
            InitializeCharacter(playerStatus, playerStatus.charCharacterData, 4); // playerStatus.charCharacterData should be set in the inpector
        }
        else
        {
            // Load game if there's a save
            if (gameManager != null)
            {
                gameManager.LoadGame(); //this loads the players in the end
            }
        }
        playerStatus.gameObject.SetActive(true);
        playerStatus.UpdateHUD();

        //enemyStatus.gameObject.SetActive(true);
    }

    public void InitializeCharacter(CharacterStatus characterStatus, CharacterData characterData, int Rolls = 4, int DiceSize = 6)
    {
        if (characterStatus != null)
        {
            // Set up the character stats using the provided CharacterStatus and CharacterData
            characterStatus.SetupCharacter(characterData, Rolls, DiceSize);

            // Set up skills for the character
            SkillComponent skillComponent = characterStatus.GetComponent<SkillComponent>();
            if (skillComponent != null)
            {
                skillComponent.SetupSkills(characterData); // Set up character's skills based on data
            }
        }
    }

    public void EndBattle(bool playerWon)
    {
        UIManager.instance.HideActionButtons();

        string message = playerWon ? $"You defeated {enemyStatus.characterName}!" : "You were defeated!";
        string nextScene = playerWon ? "BattleScene" : "MainMenu";
        string resultScreen = playerWon ? "WinScreen" : "LoseScreen";

        if (gameManager == null)
        {
            Debug.LogError("GameManager is null!");
            return;
        }

        //Handle save game for the next battle
        if (playerWon)
        {
            playerStatus.SaveToCharacterData(); //  tell the characterStatus to save its status to its characterData
            UpdateMonsterLevelProbabilities();  //  calculate level probabilities for next level 
            gameManager.OnBattleWon();          //  tell game manager to save playerData
        }
        else
        {
            gameManager.numberOfFights = 0;
            gameManager.OnBattleLost();
        }


        StartCoroutine(LoadSceneAndPerformAction(resultScreen, () =>
        {
            BattleTextManager.Instance.DisplayMessageForSeconds(message, 1, () =>
            {
                if (playerWon)
                {
                    gameManager.AddGold(10);
                    BattleTextManager.Instance.DisplayMessageForSeconds("You won 10 gold!", 1, () =>
                    {
                        gameManager.AddExperience(5);
                        BattleTextManager.Instance.DisplayMessageForSeconds("You won 5 experience!", 1, () =>
                        {
                            //check if leveled up and 

                            StartCoroutine(LoadSceneAndPerformAction(nextScene, SetupBattle));
                        });
                    });
                }
                else
                {
                    StartCoroutine(LoadSceneAndPerformAction(nextScene, () =>
                    {
                        DestroyAllDontDestroyOnLoadObjects();
                        gameManager.SetMainMenuButtons();
                    }));
                }
            });
        }));
    }

    public void SetState(BattleState newState)
    {
        currentState = newState;
        currentState.EnterState();
        //print("current state of battle " +currentState);
    }

    public IEnumerator LoadSceneAndPerformAction(string sceneName, Action afterSceneLoadAction)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        afterSceneLoadAction?.Invoke();
    }

    public void DestroyAllDontDestroyOnLoadObjects()
    {
        GameObject[] dontDestroyObjects = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
        foreach (GameObject obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }

    // Shared attack function for both player and enemy
    public void PerformAttack(CharacterStatus attacker, CharacterStatus target, string attackMessage, System.Action onAttackComplete)
    {
        BattleTextManager.Instance.DisplayMessageAndWaitEffect(attackMessage, ParticleEffectUtility.PlayParticleEffect("PS_ParticleTest"), () =>
        {
            target.TakeDamage(attacker.damage);
            BattleTextManager.Instance.DisplayMessageForSeconds($"{target.characterName} took {attacker.damage} damage.", 1.0f, () =>
            {
                onAttackComplete?.Invoke();
            });
        });
    }

       // Dictionary to store probabilities for each monster level
    private Dictionary<int, float> levelProbabilities = new Dictionary<int, float>()
    {
        { 0, 70f }, // Initial 70% for level 0
        { 1, 30f }, // Initial 30% for level 1
        { 2, 0f },  // 0% for higher levels initially
        { 3, 0f },
        { 4, 0f },
        { 5, 0f }
    };

    // Function to get a random monster level based on probabilities
    int GetRandomMonsterLevel()
    {
        float totalProbability = 0f;
        foreach (var entry in levelProbabilities)
        {
            totalProbability += entry.Value;
        }

        // Generate a random number between 0 and total probability
        float randomValue = UnityEngine.Random.Range(0, totalProbability);

        // Find which level corresponds to this random value
        float cumulativeProbability = 0f;
        foreach (var entry in levelProbabilities)
        {
            cumulativeProbability += entry.Value;
            if (randomValue <= cumulativeProbability)
            {
                return entry.Key; // Return the selected level
            }
        }

        return 0; // Default fallback to level 0
    }


    private void UpdateMonsterLevelProbabilities()
    {
        float baseIncrease = UnityEngine.Random.Range(5f, 10f);  // Base increase range
        float constant = 50f;  // Control the decay rate (you can tweak this value)
        float decayFactor = Mathf.Exp(-gameManager.numberOfFights / constant);  // Exponential decay
        float finalIncrease = baseIncrease * decayFactor;

        // Define max probabilities per level
        Dictionary<int, float> maxProbabilities = new Dictionary<int, float>
    {
        { 0, 80f },
        { 1, 70f },
        { 2, 60f },
        { 3, 50f },
        { 4, 40f },
        { 5, 30f }
    };

        // Find the first level that has not reached its maximum probability
        int levelToIncrease = -1;
        for (int i = levelThreshold; i < levelProbabilities.Count; i++) // Start at the current threshold
        {
            if (levelProbabilities[i] < maxProbabilities[i])
            {
                levelToIncrease = i;
                break; // Only increase the first level that hasn't reached its maximum
            }
        }

        // If there's no level to increase, return early
        if (levelToIncrease == -1)
        {
            Debug.Log("All levels have reached their maximum probabilities.");
            return; // Nothing to increase
        }

        // Calculate the remaining increase without exceeding the max
        float remainingIncrease = Mathf.Min(finalIncrease, maxProbabilities[levelToIncrease] - levelProbabilities[levelToIncrease]);
        levelProbabilities[levelToIncrease] += remainingIncrease;

        // If the level has reached its max, increase the threshold to move to the next level
        if (levelProbabilities[levelToIncrease] >= maxProbabilities[levelToIncrease])
        {
            levelThreshold = Mathf.Min(levelThreshold + 1, levelProbabilities.Count - 1); // Increment threshold, but don't exceed max level
        }

        // Decrease the lowest non-zero levels proportionally
        float decrease = remainingIncrease; // The same amount should be subtracted from lower levels
        for (int j = 0; j < levelToIncrease; j++) // Iterate through earlier levels
        {
            if (levelProbabilities[j] > 0)
            {
                float maxDecrease = Mathf.Min(decrease, levelProbabilities[j]);  // We can't reduce below 0
                levelProbabilities[j] -= maxDecrease;
                decrease -= maxDecrease; // Reduce remaining decrease amount
                if (decrease <= 0) break; // No more decrease needed
            }
        }

        // Ensure the probabilities are normalized and don’t exceed 100% total.
        NormalizeProbabilities();
    }

    // Normalize the probabilities to sum to 100
    private void NormalizeProbabilities()
    {
        float totalProbability = 0f;
        for (int i = 0; i < levelProbabilities.Count; i++)
        {
            totalProbability += levelProbabilities[i];
        }

        if (totalProbability > 100f) // Should never happen, but just in case
        {
            // Scale down to make the total 100
            for (int i = 0; i < levelProbabilities.Count; i++)
            {
                levelProbabilities[i] = (levelProbabilities[i] / totalProbability) * 100f;
            }
        }
    }


    // Determine the number of dice rolls based on number of fights
    int CalculateDiceRolls(int numberOfFights)
    {
        if (numberOfFights < 12)
        {
            // 70% chance of 2 rolls, 30% chance of 3 rolls
            return UnityEngine.Random.Range(0f, 1f) < 0.7f ? 2 : 3;
        }
        if (numberOfFights < 20)
        {
            // 60% chance of 3 rolls, 40% chance of 4 rolls
            return UnityEngine.Random.Range(0f, 1f) < 0.6f ? 3 : 4;
        }
        if (numberOfFights < 30)
        {
            // 50% chance of 4 rolls, 50% chance of 5 rolls
            return UnityEngine.Random.Range(0f, 1f) < 0.5f ? 4 : 5;
        }
        // Default case: 5 or 6 rolls at higher levels
        return UnityEngine.Random.Range(5, 7);
    }

    // Determine the size of the dice based on number of fights
    int CalculateDiceSize(int numberOfFights)
    {
        if (numberOfFights < 12) return 6; // Standard dice (d6)
        if (numberOfFights < 20) return 8; // Slightly stronger enemies (d8)
        if (numberOfFights < 30) return 10; // Even stronger enemies (d10)
        if (numberOfFights < 40) return 12; // Dangerous enemies (d12)
        return 20; // Endgame/boss level (d20)
    }

}