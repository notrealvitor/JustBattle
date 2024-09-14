using System.Collections;
using UnityEngine;

public class EnemyTurnState : BattleState
{
    public EnemyTurnState(CombatManager combatManager) : base(combatManager) { }

    public override void EnterState()
    {
            BattleTextManager.Instance.DisplayMessage($"{combatManager.enemyStatus.characterName} is attacking!");
            combatManager.ActionButtons.SetActive(false); // Hide buttons during enemy turn
            combatManager.StartCoroutine(EnemyAction());
            BattleTextManager.Instance.ClearMessage();
    }

    public override void UpdateState() { }

    IEnumerator EnemyAction()
    {
        yield return new WaitForSeconds(1); // Wait before enemy attacks

        BattleTextManager.Instance.DisplayMessageAndWaitEffect($"{combatManager.enemyStatus.characterName} attacks!", ParticleEffectUtility.PlayParticleEffect("PS_ParticleTest"), () =>
        {
            combatManager.playerStatus.TakeDamage(combatManager.enemyStatus.damage);
            BattleTextManager.Instance.DisplayMessageForSeconds(combatManager.playerStatus.characterName + " have suffered " + combatManager.enemyStatus.damage + " damage.", 1.0f, () =>
            {
                if (combatManager.playerStatus.IsDead())
                {
                    combatManager.EndBattle(false); // Player loses
                }
                else
                {
                    combatManager.SetState(new PlayerTurnState(combatManager)); // Back to player's turn
                }
            });

        });
    }
}