using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    //mana
    public int maxMana = 100;
    public int currentMana;
    public TMP_Text manaText;  // Reference to the UI text showing current mana

    //skills
    public string[] startingSkillsNames;  // Skill names to be loaded from Resources/Skills
    [HideInInspector] public List<SkillActionObject> availableSkills = new List<SkillActionObject>();  // Store loaded skills


    void Start()
    {
        currentMana = maxMana;
        UpdateManaDisplay();
        LoadSkills();
    }

    public void SetupSkills(CharacterData characterData = null)
    {
        if (characterData != null)
        {
            // Calculate mana for enemies
            maxMana = CalculateMaxMana(characterData);
        }
        else
        {
            // Handle player skills
            maxMana = CalculateMaxMana(null);
        }
        currentMana = maxMana;
    }

    // Function to calculate mana based on intelligence stat
    private int CalculateMaxMana(CharacterData characterData = null)
    {
        if (characterData != null)
        {
            // Calculate max mana based on enemy's intelligence
            return characterData.intelligence + characterData.level * 3;  // Example: 10 mana per intelligence point
        }
        return 10;  // Default fallback value
    }

    // Function to load skills based on skillNames array
    public void LoadSkills()
    {
        foreach (string skillName in startingSkillsNames)
        {
            // Look for the skill in the Resources/Skills folder
            SkillActionObject skill = Resources.Load<SkillActionObject>($"Skills/{skillName}");
            if (skill != null)
            {
                availableSkills.Add(skill);
                Debug.Log($"Loaded skill: {skillName}");
            }
            else
            {
                Debug.LogError($"Skill '{skillName}' not found in Resources/Skills");
            }
        }
    }

    public void UseSkill(SkillActionObject skill, CharacterStatus targetCharacter)
    {
        if (skill != null && currentMana >= skill.manaCost)
        {
            skill.Use(GetComponent<CharacterStatus>(), targetCharacter);
            currentMana -= skill.manaCost;
            UpdateManaDisplay();
        }
    }

    public void RegenerateMana(int amount)
    {
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        UpdateManaDisplay();
    }

    void UpdateManaDisplay()
    {
        if (manaText != null)
        {
            manaText.text = $" M: {currentMana} / {maxMana}";
        }
    }
}