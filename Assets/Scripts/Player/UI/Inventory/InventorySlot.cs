using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    [HideInInspector] public bool isEmpty = true;

    public UnityEvent OnClear;
    public UnityEvent OnFill;

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

    public void Fill(InventoryItem invItem, Sprite itemSprite, int newAmount)
    {
        isEmpty = false;
        inventoryItem = invItem;
        image.enabled = true;
        image.sprite = itemSprite;
        Amount = newAmount;
        
        OnFill.Invoke();
    }
    
    public void Clear()
    {
        Amount = 0;
        isEmpty = true;
        inventoryItem = null;
        image.sprite = null;
        image.enabled = false;

        OnClear.Invoke();
    }
}