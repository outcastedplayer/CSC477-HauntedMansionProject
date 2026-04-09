using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Escape Game/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public bool isKeyItem;
    public bool isConsumable;
}
