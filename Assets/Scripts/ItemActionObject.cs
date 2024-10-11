using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Actions/Item")]
public class ItemActionObject : ActionObject
{
    public int quantity;   // Quantity for consumable items

    public override void Use(CharacterStatus userStatus, CharacterStatus targetStatus)
    {
        Debug.Log($"{userStatus.characterName} uses {actionName}");
        ApplyItemEffect(userStatus);

        if (isConsumable)
        {
            quantity--;
            if (quantity <= 0)
            {
                Debug.Log($"{actionName} has been used up.");
            }
        }
    }

    private void ApplyItemEffect(CharacterStatus userStatus)
    {
        // Implement item logic (healing, stat boosts, etc.)
        Debug.Log($"Item {actionName} applied: {effect}");
    }
}