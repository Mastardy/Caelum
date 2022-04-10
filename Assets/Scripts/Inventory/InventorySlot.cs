using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : NetworkBehaviour
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
}