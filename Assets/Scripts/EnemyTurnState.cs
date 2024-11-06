using System.Collections;
using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(CombatManager combatManager) : base(combatManager) { }

    public override void EnterState()
    {
        Debug.Log("EnterState EnemyTurn");
        BattleTextManager.Instance.DisplayMessage($"{combatManager.enemyStatus.characterName} is attacking!");
        combatManager.StartCoroutine(EnemyAction());
        BattleTextManager.Instance.ClearMessage();
        UIManager.instance.HideActionButtons();
        UIManager.instance.HideActionsList();
    }

    public override void UpdateState() { }

    IEnumerator EnemyAction()
    {
        yield return new WaitForSeconds(1); // Wait before enemy attacks
        // Use PerformAttack method in CombatManager to handle the enemy attack
        combatManager.PerformAttack(
            combatManager.enemyStatus,  // Attacker
            combatManager.playerStatus, // Target
            $"{combatManager.enemyStatus.characterName} attacks!",  // Attack message
            () =>
            {
                // Callback after the attack: Check if player is dead or continue to the player's turn
                if (combatManager.playerStatus.IsDead())
                {
                    combatManager.EndBattle(false); // Player loses
                }
                else
                {
                    combatManager.SetState(new PlayerTurnState(combatManager)); // Switch back to player's turn
                }
            }
        );
    }
}