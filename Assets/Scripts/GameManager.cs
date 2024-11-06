using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { StartMenu, NewGame, Continue, GameOver }
    public bool ShouldLoadSavedData;

    public int numberOfFights;

    public GameState currentState;

    public static GameManager instance;         // Singleton instance

    private CharacterStatus playerStatus;       // Reference to CharacterStatus
    public PlayerData playerSaveData;           // Holds the player's save data

    private Button startButton;
    private Button continueButton;
    private Button settingsButton;
    private Button quitButton;


    void Awake()
    {
        numberOfFights = 1;
        SetMainMenuButtons();

        // Implement Singleton pattern
        if (instance == null)
        {
            instance = this; // Set the instance to this instance of the GameManager
            DontDestroyOnLoad(this.gameObject); // Ensure the GameManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GameManager objects
        }

    }

    public void ChangeState(GameState newState) // load scenes
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.StartMenu:
                SceneManager.LoadSceneAsync("MainMenu");
                break;
            case GameState.NewGame:
                DeleteSaveGame();
                SceneManager.LoadSceneAsync("BattleScene");
                break;
            case GameState.Continue:
                SceneManager.LoadSceneAsync("BattleScene");
                break;
            case GameState.GameOver:
                                // If we are running in a standalone build of the game
                #if UNITY_STANDALONE
                                Application.Quit();
                #endif

                                // If we are running in the editor
                #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                #endif
                break;
        }
    }

    public void SetMainMenuButtons()
    {
        // Find the Canvas in the scene and then locate buttons inside it
        Canvas canvas = GameObject.FindObjectOfType<Canvas>(); // Finds the active Canvas in the scene
        if (canvas != null)
        {
            startButton = canvas.transform.Find("StartBTN")?.GetComponent<Button>();
            continueButton = canvas.transform.Find("ContinueBTN")?.GetComponent<Button>();
            settingsButton = canvas.transform.Find("SettingsBTN")?.GetComponent<Button>();
            quitButton = canvas.transform.Find("QuitBTN")?.GetComponent<Button>();
        }

        // Assign actions to buttons
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        ChangeState(GameState.NewGame);
        
    }

    public void ContinueGame()
    {
        ChangeState(GameState.Continue);
        
    }

    public void OpenSettings()
    {        

    }

    public void QuitGame()
    {
        print("quit");
        ChangeState(GameState.GameOver);
    }

    // Initialization
    // Initialization
    private void InitializePlayerData()
    {
        if (playerStatus == null) playerStatus = GameObject.FindWithTag("Player")?.GetComponent<CharacterStatus>();

        if (playerStatus != null)
        {
            // If no save data, create new PlayerData based on current CharacterStatus
            if (playerSaveData == null)
            {
                //playerSaveData = SaveSystem.LoadFromFile() ?? new PlayerData(
                //    playerStatus.experience,
                //    999, // Default gold value, this should be set when monetization is done
                //    2, // Current number of fights, should start at two because if it is saved means one fight was already won
                //    playerStatus.charCharacterData // Reference the player's CharacterData
                //);

                //ApplySaveDataToCharacter();
            }
            else
            {
                // Update save data with the latest player stats
                playerSaveData.experience = playerStatus.experience;
                playerSaveData.numberOfFights = numberOfFights;
                playerSaveData.playerCharacterData = playerStatus.charCharacterData;
            }

            //ApplySaveDataToCharacter(); isnt that in the load only?
        }
    }

    public void SaveGame()
    {
        InitializePlayerData();
        if (playerStatus != null)
        {
            SaveSystem.SaveToFile(playerSaveData);
            Debug.Log($"Game SAVED: Health={playerSaveData.playerCharacterData.health}, Experience={playerSaveData.experience}, Gold={playerSaveData.gold}, NumberOfFights={playerSaveData.numberOfFights}");
        }
    }

    public void LoadGame()
    {
        playerSaveData = SaveSystem.LoadFromFile();
        //Debug.Log("loading game!");
        if (playerSaveData != null)
        {
            ApplySaveDataToCharacter();
            Debug.Log($"Game LOADED: Health={playerSaveData.playerCharacterData.health}, Experience={playerSaveData.experience}, Gold={playerSaveData.gold}, NumberOfFights={playerSaveData.numberOfFights}");
        }
    }

    private void ApplySaveDataToCharacter()
    {
        if (playerStatus == null) playerStatus = FindObjectOfType<CharacterStatus>();

        if (playerStatus != null)
        {
            // Apply save data to the CharacterStatus
            playerStatus.experience = playerSaveData.experience;
            numberOfFights = playerSaveData.numberOfFights;
            //could get the gold here

            playerStatus.charCharacterData = playerSaveData.playerCharacterData;
            playerStatus.LoadCharacterData();
        }
    }

    public void DeleteSaveGame()
    {
        SaveSystem.DeleteSaveFile();
    }

    public void AddExperience(int amount)
    {
        if (playerSaveData != null)
        {
            playerSaveData.experience += amount;
            if (playerStatus != null) playerStatus.experience = playerSaveData.experience; // Update CharacterStatus as well
            Debug.Log("Added " + amount + " experience. New total: " + playerSaveData.experience);
        }
    }

    // New Function: Increase gold
    public void AddGold(int amount)
    {
        if (playerSaveData != null)
        {
            playerSaveData.gold += amount;
            Debug.Log("Added " + amount + " gold. New total: " + playerSaveData.gold);
        }
    }

    public void OnBattleWon()
    {
        numberOfFights++;
        SaveGame();
    }

    public void OnBattleLost()
    {
        //numberOfFights = 1; I dont think we need to set this since it will be deleted anyway
        DeleteSaveGame();
    }
}
