using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { StartMenu, NewGame, Continue, GameOver }
    public bool ShouldLoadSavedData;

    public int numberOfFights;

    public GameState currentState;

    public static GameManager instance; // Singleton instance

    private CharacterStatus characterStatus;  // Reference to CharacterStatus
    public PlayerSaveData playerSaveData;     // Holds the player's save data

    void Awake()
    {
        numberOfFights = 1;
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

    public void ChangeState(GameState newState)
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

    public void StartGame()
    {
        ChangeState(GameState.NewGame);
    }

    public void ContinueGame()
    {
        ChangeState(GameState.Continue);
    }

    public void QuitGame()
    {
        print("quit");
        ChangeState(GameState.GameOver);
    }

    private void InitializePlayerData()
    {
        if (characterStatus == null) characterStatus = FindObjectOfType<CharacterStatus>();

        if (characterStatus != null) // if its null we need to create a player save data
        {
            if (playerSaveData == null)
            {
                playerSaveData = SaveSystem.LoadFromFile() ?? new PlayerSaveData(
                characterStatus.GetHealth(),
                characterStatus.healthMax,
                characterStatus.experience,
                999, // Default gold value
                1    //if there was no saveData then its 0
                );
            }
            else 
            {
                playerSaveData.health = characterStatus.GetHealth();
                playerSaveData.maxHealth = characterStatus.healthMax;
                playerSaveData.experience = characterStatus.experience;
                playerSaveData.numberOfFights = numberOfFights;
            }
            ApplySaveDataToCharacter();
        }
    }

    public void SaveGame()
    {
        InitializePlayerData();
        if (characterStatus != null)
        {
            SaveSystem.SaveToFile(playerSaveData);
            Debug.Log($"Game SAVED: Health={playerSaveData.health}, MaxHealth={playerSaveData.maxHealth}, Experience={playerSaveData.experience}, Gold={playerSaveData.gold}, NumberOfFights={playerSaveData.numberOfFights}");
        }
    }

    public void LoadGame()
    {        
        playerSaveData = SaveSystem.LoadFromFile();

        if (playerSaveData != null)
        {
            ApplySaveDataToCharacter();
            Debug.Log($"Game LOADED: Health={playerSaveData.health}, MaxHealth={playerSaveData.maxHealth}, Experience={playerSaveData.experience}, Gold={playerSaveData.gold}, NumberOfFights={playerSaveData.numberOfFights}");
        }
    }

    private void ApplySaveDataToCharacter()
    {
        if (characterStatus == null) characterStatus = FindObjectOfType<CharacterStatus>();
        
        if (characterStatus != null)
        {
            characterStatus.SetHealth(playerSaveData.health);
            characterStatus.healthMax = playerSaveData.maxHealth;
            characterStatus.experience = playerSaveData.experience;
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
            if (characterStatus != null) characterStatus.experience = playerSaveData.experience; // Update CharacterStatus as well
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
    }

}
