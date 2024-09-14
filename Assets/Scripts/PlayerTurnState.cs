using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnState : BattleState
{
    public PlayerTurnState(CombatManager combatManager) : base(combatManager) { }

    [HideInInspector]
    public Button attackButton;

    public override void EnterState()
    {
       // combatManager.dialogueText.text = "Your turn! Choose an action.";
        BattleTextManager.Instance.DisplayMessage("Your turn! Choose an action.");
        combatManager.ActionButtons.SetActive(true); // Enable player's action buttons



        combatManager.ClearOnAttackEvent();
        combatManager.onAttack += PlayerAttack;
        

    }

    public override void UpdateState()
    {
        // If the player performs an attack, call PlayerAttack
        if (Input.GetKeyDown(KeyCode.Space)) // Replace with actual player action
        {
            combatManager.ActionButtons.SetActive(false); // Disable buttons after action
            PlayerAttack();
        }
    }

    private void PlayerAttack()
    {
        combatManager.ActionButtons.SetActive(false); // Disable buttons after action
        BattleTextManager.Instance.DisplayMessageAndWaitEffect($"{combatManager.playerStatus.characterName} attacks!", ParticleEffectUtility.PlayParticleEffect("PS_ParticleTest"), () =>
        {
            combatManager.EnemyTakeDamage(combatManager.playerStatus.damage);
            BattleTextManager.Instance.DisplayMessageForSeconds(combatManager.enemyStatus.characterName + " have suffered " + combatManager.playerStatus.damage + " damage.", 1.0f, () =>
            {
                if (combatManager.enemyStatus.IsDead())
                {
                    combatManager.dialogueText.text = $"You defeated {combatManager.enemyStatus.characterName}!";
                    combatManager.EndBattle(true); // Player won
                }
                else
                {
                    combatManager.SetState(new EnemyTurnState(combatManager)); // Switch to Enemy turn
                }
            });
        });
    }


}