using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public InventorySlot slot;
    [SerializeField] private Image image;
    [SerializeField] private GameObject amount;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject durability;
    [SerializeField] private Image durabilityForeground;

    private void Start()
    {
        slot.OnClear.AddListener(OnClear);
        slot.OnFill.AddListener(OnFill);
        slot.OnDurabilityChange.AddListener(() => durabilityForeground.fillAmount = 0.5f * slot.Durability);
        slot.OnAmountChange.AddListener(() => amountText.SetText(slot.Amount.ToString()));
    }

    public void OnClear()
    {
        image.enabled = false;
        image.sprite = null;
        durability.SetActive(false);
        amount.SetActive(false);
    }

    public void OnFill()
    {
        image.enabled = true;
        image.sprite = slot.image.sprite;
        if (slot.inventoryItem.itemTag is ItemTag.Axe or ItemTag.Pickaxe or ItemTag.Weapon or ItemTag.RangeWeapon)
        {
            durability.SetActive(true);
            durabilityForeground.fillAmount = 0.5f * slot.Durability;
        }
        else
        {
            amount.SetActive(true);
            amountText.SetText(slot.Amount.ToString());
        }
    }

    private void OnDurabilityChanged()
    {
        
    }
}