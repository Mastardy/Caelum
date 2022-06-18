using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public InventorySlot slot;
    
    [SerializeField] private Image image;
    [SerializeField] private Image background;
    [SerializeField] private GameObject amount;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject durability;
    [SerializeField] private Gradient durabilityGradient;
    [SerializeField] private Image durabilityForeground;

    private Image backgroundImage;
    
    private Color slotSelected = new(1f, 0.85f, 0.65f, 0.5f);
    private Color slotUnselected = new(0.8f, 0.7f, 0.6f, 0.5f);
    private float lastSelected;

    private bool selected;
    public bool Selected
    {
        get => selected;
        set
        {
            if(value != selected) lastSelected = Time.time;
            selected = value;
        }
    }

    private void Start()
    {
        backgroundImage = GetComponent<Image>();
        slot.OnClear.AddListener(OnClear);
        slot.OnFill.AddListener(OnFill);
        slot.OnDurabilityChange.AddListener(() =>
        {
            durabilityForeground.fillAmount = 0.5f * slot.Durability;
            durabilityForeground.color = durabilityGradient.Evaluate((slot.Durability * -1) + 1);
        });
        slot.OnAmountChange.AddListener(() => amountText.SetText(slot.Amount.ToString()));
    }

    private void Update()
    {
        var lastSelectedAdjusted = (Time.time - lastSelected) * 5f;

        if (lastSelectedAdjusted > 1) return;
        
        if (selected)
        {
            background.color = Color.Lerp(slotUnselected, slotSelected, lastSelectedAdjusted);
            backgroundImage.color = Color.Lerp(slotUnselected, slotSelected, lastSelectedAdjusted);
            var tempValue = Mathf.Lerp(1.0f, 1.1f, lastSelectedAdjusted);
            transform.localScale = new Vector3(tempValue, tempValue);
        }
        else
        {
            background.color = Color.Lerp(slotSelected, slotUnselected, lastSelectedAdjusted);
            backgroundImage.color = Color.Lerp(slotSelected, slotUnselected, lastSelectedAdjusted);
            var tempValue = Mathf.Lerp(1.1f, 1.0f, lastSelectedAdjusted);
            transform.localScale = new Vector3(tempValue, tempValue);
        }
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
        if (slot.inventoryItem.itemTag is not ItemTag.Food and not ItemTag.Other)
        {
            durability.SetActive(true);
            durabilityForeground.fillAmount = 0.5f * slot.Durability;
            durabilityForeground.color = durabilityGradient.Evaluate((slot.Durability * -1) + 1);
        }
        else
        {
            amount.SetActive(true);
            amountText.SetText(slot.Amount.ToString());
        }
    }
}