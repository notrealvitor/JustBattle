using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "SkillData", menuName = "Actions/Skill")]
public class SkillActionObject : ActionObject
{
    public int manaCost;
    public float cooldown;
    public string skillType;  // e.g., "physical", "fire", "water"
    private CombatManager combatManager;
    public float dmgMultiplier = 1;

    public override void Use(CharacterStatus userStatus, CharacterStatus targetStatus)
    {
        SkillComponent skillComponent = userStatus.GetComponent<SkillComponent>();
        combatManager = FindObjectOfType<CombatManager>();
        

        if (skillComponent != null && skillComponent.currentMana >= manaCost)
        {
            ApplySkillEffect(userStatus, targetStatus);
            skillComponent.currentMana -= manaCost;  // Reduce mana from SkillComponent
            string attackMessage = $"{userStatus.characterName} uses {actionName}, costing {manaCost} mana.";
            UIManager.instance.HideActionButtons();
            UIManager.instance.HideActionsList();

            BattleTextManager.Instance.DisplayMessageAndWaitEffect(attackMessage, ParticleEffectUtility.PlayParticleEffect("PS_ParticleTest"), () =>
            {
                
                targetStatus.TakeDamage(Mathf.RoundToInt(userStatus.damage * dmgMultiplier));
                ApplySkillEffect(userStatus, targetStatus);
                BattleTextManager.Instance.DisplayMessageForSeconds($"{targetStatus.characterName} took {Mathf.RoundToInt(userStatus.damage * dmgMultiplier)} damage.", 1.0f, () =>
                {
                    //combatManager.SetState(new EnemyTurnState(combatManager));
 
                    combatManager.turnManager.NextTurn();
                });
            });
        }
        else
        {
            Debug.Log("Not enough mana!");
        }
    }



    private void ApplySkillEffect(CharacterStatus userStatus, CharacterStatus targetStatus)
    {
        // Implement skill logic (damage, healing, status effect)
        Debug.Log($"Skill {actionName} applied: {effect}");
    }
}