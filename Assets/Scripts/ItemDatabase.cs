using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Actions/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public ItemActionObject[] storedItems; // Array to store all items

    // Function to get a specific item by name
    public ItemActionObject GetItemByName(string itemName)
    {
        foreach (ItemActionObject item in storedItems)
        {
            if (item.actionName == itemName)
            {
                return item;
            }
        }

        Debug.LogError($"Item '{itemName}' not found in the database!");
        return null;
    }
}