using UnityEngine;

public abstract class ActionObject : ScriptableObject
{
    public string actionName;    // Common name for both skills and items
    public string effect;        // Description of the effect (could be damage, healing, etc.)
    public bool isConsumable;    // Whether the action is consumable (for items)

    public abstract void Use(CharacterStatus userStatus, CharacterStatus targetStatus);
}