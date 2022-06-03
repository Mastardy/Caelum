using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot : MonoBehaviour
{
    [HideInInspector] public bool isEmpty = true;

    public UnityEvent OnClear;
    public UnityEvent OnFill;
    public UnityEvent OnDurabilityChange;
    public UnityEvent OnAmountChange;
    
    [SerializeField] private GameObject amountObject;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject durabilityObject;
    [SerializeField] private Gradient durabilityGradient;
    [SerializeField] private Image durabilityForeground;
    
    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            amount = value;
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

    private void Start()
    {
        OnDurabilityChange.AddListener(() =>
        {
            durabilityForeground.fillAmount = 0.5f * durability;
            durabilityForeground.color = durabilityGradient.Evaluate((durability * -1) + 1);
        });
        
        OnAmountChange.AddListener(
            () => {
                amountText.SetText(amount.ToString());
                Debug.Log("Teste");
            });
    }

    public void Fill(InventoryItem invItem, int newAmount, float newDurability)
    {
        isEmpty = false;
        inventoryItem = invItem;
        image.enabled = true;
        image.sprite = invItem.sprite;
        Amount = newAmount;
        durability = newDurability;

        if (inventoryItem.itemTag is not ItemTag.Food and not ItemTag.Grappling and not ItemTag.Other)
        {
            durabilityObject.SetActive(true);
            durabilityForeground.fillAmount = 0.5f * durability;
            durabilityForeground.color = durabilityGradient.Evaluate((durability * -1) + 1);
        }
        else
        {
            amountObject.SetActive(true);
            amountText.SetText(Amount.ToString());
        }
        
        OnFill.Invoke();
    }
    
    public void Clear()
    {
        durabilityObject.SetActive(false);
        amountObject.SetActive(false);
        
        Amount = 0;
        isEmpty = true;
        inventoryItem = null;
        image.sprite = null;
        image.enabled = false;
        durability = 0;

        OnClear.Invoke();
    }
}