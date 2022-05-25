using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public InventorySlot slot;
    public Outline outline;
    
    [SerializeField] private Image image;
    [SerializeField] private GameObject amount;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject durability;
    [SerializeField] private Gradient durabilityGradient;
    [SerializeField] private Image durabilityForeground;

    private Image backgroundImage;
    
    private Color slotSelected = new(0.85f, 0.75f, 0.65f);
    private Color slotUnselected = new(0.95f, 0.8f, 0.6f);
    private Color outlineSelected = new(0.95f, 0.9f, 0.9f);
    private Color outlineUnselected = new(0.65f, 0.55f, 0.4f);
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
        slot.OnDurabilityChange.AddListener(() => durabilityForeground.fillAmount = 0.5f * slot.Durability);
        slot.OnAmountChange.AddListener(() => amountText.SetText(slot.Amount.ToString()));
    }

    private void Update()
    {
        var lastSelectedAdjusted = (Time.time - lastSelected) * 5f;
        
        if (selected)
        {
            outline.effectColor = Color.Lerp(outlineUnselected, outlineSelected, lastSelectedAdjusted);
            backgroundImage.color = Color.Lerp(slotUnselected, slotSelected, lastSelectedAdjusted);
            var tempValue = Mathf.Lerp(1.0f, 1.1f, lastSelectedAdjusted);
            transform.localScale = new Vector3(tempValue, tempValue);
        }
        else
        {
            outline.effectColor = Color.Lerp(outlineSelected, outlineUnselected, lastSelectedAdjusted);
            backgroundImage.color = Color.Lerp(slotSelected, slotUnselected, lastSelectedAdjusted);
            var tempValue = Mathf.Lerp(1.1f, 1.0f, lastSelectedAdjusted);
            transform.localScale = new Vector3(tempValue, tempValue);
        }
        
        if (slot.isEmpty) return;
        if (slot.inventoryItem.itemTag is not ItemTag.Food or ItemTag.Grappling or ItemTag.Other)
        {
            durabilityForeground.color = durabilityGradient.Evaluate((slot.Durability*-1) + 1);
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
        if (slot.inventoryItem.itemTag is not ItemTag.Food or ItemTag.Grappling or ItemTag.Other)
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