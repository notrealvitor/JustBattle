using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private CombatManager combatManager;

    void Start()
    {
        // Find the CombatManager and set the initial state
        combatManager = FindObjectOfType<CombatManager>();
        combatManager.SetState(new PlayerTurnState(combatManager)); // Start with player's turn
    }

    // This method can be called to switch turns (between player and enemy)
    public void SwitchToEnemyTurn()
    {
        combatManager.SetState(new EnemyTurnState(combatManager));
    }

    public void SwitchToPlayerTurn()
    {
        combatManager.SetState(new PlayerTurnState(combatManager));
    }
}