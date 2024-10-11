using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnState : BattleState
{
    public PlayerTurnState(CombatManager combatManager) : base(combatManager) { }

    [HideInInspector]
    public Button attackButton;

    public override void EnterState()
    {
        Debug.Log("EnterState PlayerTurn");
        // Display the player's turn message
        BattleTextManager.Instance.DisplayMessage("Your turn! Choose an action.");

        // Enable the player's action buttons through UIManager
        UIManager.instance.ShowActionButtons();
    }

    public override void UpdateState()
    {

    }

    public void PlayerAttack()
    {
        UIManager.instance.HideActionButtons();
        UIManager.instance.HideActionsList();

        // Use CombatManager to handle the attack
        CombatManager.instance.PerformAttack(
            CombatManager.instance.playerStatus,
            CombatManager.instance.enemyStatus,
            $"{CombatManager.instance.playerStatus.characterName} attacksss!",
            () =>
            {
                if (CombatManager.instance.enemyStatus.IsDead())
                {
                    CombatManager.instance.EndBattle(true); // Player won
                }
                else
                {
                    CombatManager.instance.SetState(new EnemyTurnState(CombatManager.instance)); // Switch to enemy turn
                }
            }
        );
    }
}