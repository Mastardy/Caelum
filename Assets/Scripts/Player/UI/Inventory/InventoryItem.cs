using UnityEngine;

public enum ItemTag
{
    Other,
    Armor,
    Food,
    Axe,
    Pickaxe,
    Sword,
    Spear,
    Bow,
    Grappling
}

public enum SubTag
{
    None,
    Food,
    Drink,
    Seed
}

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public ItemTag itemTag;
    public SubTag subTag;
    
    public Sprite sprite;
    [TextArea(1, 10)]
    public string description;
    public int maxStack;

    public GameObject worldPrefab;
}