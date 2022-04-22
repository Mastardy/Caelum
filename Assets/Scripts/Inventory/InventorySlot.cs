using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    [HideInInspector] public bool isEmpty = true;

    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            text.SetText(value == 0 ? "" : value + "x");
        }
    }

    [HideInInspector] public InventoryItem inventoryItem;
    
    public Image image;
    public TextMeshProUGUI text;

    public void Clear()
    {
        Amount = 0;
        isEmpty = true;
        inventoryItem = null;
        image.sprite = null;
        image.enabled = false;
    }
}