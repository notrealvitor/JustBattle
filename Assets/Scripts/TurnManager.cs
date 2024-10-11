using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTurnManager", menuName = "Managers/TurnManager")]
public class TurnManager : ScriptableObject
{
    private CombatManager combatManager;

    private List<CharacterStatus> allCharacters; // Store all player/enemy characters
    //private int currentTurnIndex = 0; // Track whose turn it is
    public void StartTurnManager()
    {
        // Find the CombatManager and set the initial state
        combatManager = FindObjectOfType<CombatManager>();
        allCharacters = new List<CharacterStatus>(FindObjectsOfType<CharacterStatus>());
        ResetAllSpeeds();
        Debug.Log("start turn manager");
    }

    void ResetAllSpeeds()
    {
        // Iterate through all characters in the list and set their speed to 0
        foreach (CharacterStatus character in allCharacters)
        {
            character.SetSpeed(0);
        }
    }

    public void NextTurn()
    {
        Debug.Log("called NextTurn");
        // Keep increasing speed until a character has enough speed to act (speed >= 3)
        while (true)
        {
            List<CharacterStatus> readyToAct = new List<CharacterStatus>();

            if (allCharacters == null || allCharacters.Count == 0)
            {
                Debug.LogError("No characters found in the scene!");
                return; // Exit the method if no characters were found
            }

            // Check for characters with speed >= 3
            foreach (CharacterStatus character in allCharacters)
            {
               if (character.speed >= 3)
                {
                    readyToAct.Add(character);                   
                }
            }

            // If one or more characters are ready to act
            if (readyToAct.Count > 0)
            {
                // Sort by speed (highest speed goes first)
                readyToAct.Sort((a, b) => b.GetSpeed().CompareTo(a.GetSpeed()));
                CharacterStatus currentCharacter = readyToAct[0]; // The character with the highest speed acts first

                // Set the turn based on whether the current character is a player or an enemy
                if (currentCharacter.isPlayer)
                {

                    combatManager.SetState(new PlayerTurnState(combatManager));
                    Debug.Log("Player turn selected!");
                }
                else
                {
                    combatManager.SetState(new EnemyTurnState(combatManager));
                    Debug.Log("Enemy turn selected!");
                }

                // Reduce the speed of the acting character by 3 (reset after action) // this should be done after calling a skill but we can leave here for now
                currentCharacter.SetSpeed(currentCharacter.GetSpeed() - 3);

                // Exit the function as we have found a character ready to act
                return;
            }

            // No character is ready to act, so increment speed for all characters
            foreach (CharacterStatus character in allCharacters)
            {
                
                float speedBonus = 1 + character.charCharacterData.dexterity / 100f;
                character.SetSpeed(character.GetSpeed() + speedBonus);
                //Debug.Log(speedBonus + "+ speed for " + character + " goes to " + character.speed);
            }
        }
    }
}
