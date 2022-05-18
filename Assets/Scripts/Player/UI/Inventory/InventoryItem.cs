using UnityEngine;

public enum ItemTag
{
    Other,
    Food,
    Axe,
    Pickaxe,
    Sword,
    Spear,
    Bow
}

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public ItemTag itemTag;
    
    public Sprite sprite;
    [TextArea(1, 10)]
    public string description;
    public int maxStack;

    public GameObject worldPrefab;
}