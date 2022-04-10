using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : NetworkBehaviour
{
    [HideInInspector] public bool isEmpty = true;
    [HideInInspector] public int amount;
    [HideInInspector] public InventoryItem inventoryItem;
    
    public Image image;
}