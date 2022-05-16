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
    public UnityEvent OnDurabilityChange;
    public UnityEvent OnAmountChange;
    
    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
            text.SetText(value == 0 ? "" : value + "x");
            OnAmountChange.Invoke();
        }
    }

    private float durability;

    public float Durability
    {
        get => durability;
        set
        {
            durability = value;
            OnDurabilityChange.Invoke();            
        }
    }
    
    [HideInInspector] public InventoryItem inventoryItem;
    
    public Image image;
    public TextMeshProUGUI text;

    public void Fill(InventoryItem invItem, Sprite itemSprite, int newAmount, float newDurability)
    {
        isEmpty = false;
        inventoryItem = invItem;
        image.enabled = true;
        image.sprite = itemSprite;
        Amount = newAmount;
        durability = newDurability;
        
        OnFill.Invoke();
    }
    
    public void Clear()
    {
        Amount = 0;
        isEmpty = true;
        inventoryItem = null;
        image.sprite = null;
        image.enabled = false;
        durability = 0;

        OnClear.Invoke();
    }
}