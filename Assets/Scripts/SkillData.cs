using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;    // Name of the skill
    public string skillType;    // Type of the skill (e.g., "physical", "fire", "water")
    public int manaCost;        // Mana cost for the skill
    public float cooldown;      // Cooldown time for the skill
    public string effect;       // Description of the skill's effect
}
