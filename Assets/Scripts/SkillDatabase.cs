using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Actions/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public SkillActionObject[] storedSkills; // Array to store all skills

    // Function to get a specific skill by name
    public SkillActionObject GetSkillByName(string skillName)
    {
        foreach (SkillActionObject skill in storedSkills)
        {
            if (skill.actionName == skillName)
            {
                return skill;
            }
        }

        Debug.LogError($"Skill '{skillName}' not found in the database!");
        return null;
    }
}