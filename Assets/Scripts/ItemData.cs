using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;     // Name of the item
    public bool isConsumable;   // Whether the item is consumable
    public int quantity;        // Quantity of the item
    public string effect;       // Description of the item's effect
}