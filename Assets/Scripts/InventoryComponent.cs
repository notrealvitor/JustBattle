using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    //mana
    public int maxMana = 100;
    public int currentMana;
    public TMP_Text manaText;  // Reference to the UI text showing current mana

    //skills
    public string[] startingItemNames;  // Skill names to be loaded from Resources/Skills
    public List<ItemActionObject> availableItems = new List<ItemActionObject>();  // Store loaded skills


    void Start()
    {
        
    }

    // Function to load skills based on skillNames array
    public void LoadItems()
    {
       
    }

    public void UseItem(ItemActionObject itemObject, CharacterStatus targetCharacter)
    {
       print("use" + itemObject + " from " + targetCharacter);

    }
}