using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { StartMenu, NewGame, GameOver }

    public GameState currentState;

    void Start()
    {
        
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


    public void QuitGame()
    {
        print("quit");
        ChangeState(GameState.GameOver);
    }
}
