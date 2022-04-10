using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public int id;
    public string itemName;

    public Sprite sprite;
    public string description;
    public int maxStack;

    public GameObject worldPrefab;
}