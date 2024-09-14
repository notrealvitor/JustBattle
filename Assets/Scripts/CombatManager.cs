using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.XR;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Database")]
    public EnemyDatabase enemyDatabase;

    [HideInInspector]
    public GameObject playerPrefab;
    [HideInInspector]
    public GameObject enemyPrefab;
    [HideInInspector]
    public TMP_Text dialogueText;
    [HideInInspector]
    public GameObject ActionButtons;
    [HideInInspector]
    public Button attackButton;
    public delegate void OnAttackAction();
    public event OnAttackAction onAttack;

    [HideInInspector]
    public GameObject playerHUD;
    [HideInInspector]
    public GameObject enemyHUD;

    [HideInInspector]
    public CharacterStatus playerStatus;
    [HideInInspector]
    public CharacterStatus enemyStatus;

    private BattleState currentState;

    private GameManager gameManager;

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
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        enemyDatabase.StoreEnemiesFromSheet("EnemiesSheet1");
        SetupBattle();
    }

    void Update()
    {   
       if (currentState != null) 
       {
       currentState.UpdateState();
       }
    }

    // Dynamically assign references using tags
    void FindReferences()
    {
        playerPrefab = GameObject.FindWithTag("Player");
        enemyPrefab = GameObject.FindWithTag("Enemy");
        dialogueText = GameObject.FindWithTag("DialogueText")?.GetComponent<TMP_Text>();
        ActionButtons = GameObject.FindWithTag("ActionButtons");
        playerHUD = GameObject.FindObjectOfType<Canvas>().transform.Find("PlayerHUD")?.gameObject;
        enemyHUD = GameObject.FindObjectOfType<Canvas>().transform.Find("EnemyHUD")?.gameObject;

        // Check for nulls to avoid issues
        if (playerHUD == null) Debug.LogError("Player CharacterHUD not found!");
        if (enemyHUD == null) Debug.LogError("Enemy CharacterHUD not found!");

        // Get CharacterStatus components
        if (playerPrefab != null)
        {
            playerStatus = playerPrefab.GetComponent<CharacterStatus>();
        }

        if (enemyPrefab != null)
        {
            enemyStatus = enemyPrefab.GetComponent<CharacterStatus>();
        }

        // Assign attack button
        if (ActionButtons != null)
        {
            attackButton = ActionButtons.transform.Find("AttackBTN")?.GetComponent<Button>();
            if (attackButton != null)
            {
                attackButton.onClick.RemoveAllListeners(); // Clear previous listeners
                attackButton.onClick.AddListener(() => onAttack?.Invoke());
            }
        }
    }

    public void ClearOnAttackEvent()
    {
        onAttack = null; // This is allowed inside the class that declares the event
    }

    void SetupBattle()
    {
        if (gameManager != null)
        {
            if (gameManager.ShouldLoadSavedData == true) { gameManager.LoadGame(); }
            gameManager.ShouldLoadSavedData = true;
        }

        FindReferences();  // Ensure references are dynamically assigned after scene load

        SetupEnemy();
        SetupPlayer();

        if (dialogueText != null)
        {
            BattleTextManager.Instance.DisplayMessageForSeconds($"A {enemyStatus.characterName} appeared!", 1.0f, () =>
            {
                SetState(new PlayerTurnState(this));
            });
        }

        if (ActionButtons != null)
        {
            ActionButtons.SetActive(false); // Hide action buttons until battle starts
        }
    }

    void SetupEnemy()
    {
        EnemyData randomEnemy = enemyDatabase.GetRandomStoredEnemy();
        if (randomEnemy == null)
        {
            Debug.LogError("No enemy data available!");
            return;
        }

        enemyStatus.SetupCharacter(randomEnemy.enemyName, randomEnemy.enemySprite, randomEnemy.healthMax, randomEnemy.damage);
        enemyPrefab.SetActive(true);
    }

    void SetupPlayer()
    {
        playerHUD.SetActive(true);
        enemyHUD.SetActive(true);
    }

    public void EnemyTakeDamage(int damage)
    {
        enemyStatus.TakeDamage(damage);
        dialogueText.text = $"You attacked {enemyStatus.characterName} for {damage} damage!";

    }

    public void EndBattle(bool playerWon)
    {
        ActionButtons.SetActive(false);

        string message = playerWon ? $"You defeated {enemyStatus.characterName}!" : "You were defeated!";
        string resultScene = playerWon ? "WinScreen" : "LoseScreen";
        string nextScene = playerWon ? "BattleScene" : "MainMenu";


        if (playerWon && gameManager != null)
        {
            gameManager.SaveGame();
        }
        else
        {
            gameManager.DeleteSaveGame();
        }

        StartCoroutine(LoadSceneAndPerformAction(resultScene, () =>
        {
            BattleTextManager.Instance.DisplayMessageWithInput(message, () =>
            {
                if (playerWon)
                {
                    gameManager.AddGold(10); //this goes straight to the saveGame
                    BattleTextManager.Instance.DisplayMessageForSeconds("You won 10 gold!", 1,() =>
                    {
                        gameManager.AddExperience(5); //this goes straight to the saveGame
                        BattleTextManager.Instance.DisplayMessageForSeconds("You won 5 experience!", 1, () =>
                        {
                            StartCoroutine(LoadSceneAndPerformAction(nextScene, () =>
                            {
                                SetupBattle();
                            }));
                        });
                    });
                }
                else
                    StartCoroutine(LoadSceneAndPerformAction(nextScene, () =>
                    {
                        DestroyAllDontDestroyOnLoadObjects();
                    }));
            });
        }));
    }

    IEnumerator LoadSceneAndShowText(string sceneName, string message)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        BattleTextManager.Instance.DisplayMessageForSeconds(message, 1.0f, () =>
        {
            //need to load a level here and after loading need to run an action, just like in the previous one so maybe we can make a function that would be a fit for both cases
        });
    }


    public void SetState(BattleState newState)
    {
        currentState = newState;
        currentState.EnterState();
    }

    public IEnumerator LoadSceneAndPerformAction(string sceneName, Action afterSceneLoadAction)
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // After the scene has loaded, execute the action
        afterSceneLoadAction?.Invoke();
    }

    public void DestroyAllDontDestroyOnLoadObjects() //need to add the DontDestroyOnLoadTag in the object
    {
        // Find all objects tagged with "DontDestroyOnLoad"
        GameObject[] dontDestroyObjects = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");

        // Loop through and destroy each object
        foreach (GameObject obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }
}