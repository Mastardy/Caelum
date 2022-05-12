using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public InventorySlot slot;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        slot.OnClear.AddListener(OnClear);
        slot.OnFill.AddListener(OnFill);
    }

    public void OnClear()
    {
        image.enabled = false;
        image.sprite = null;
        text.SetText(string.Empty);
    }

    public void OnFill()
    {
        image.enabled = true;
        image.sprite = slot.image.sprite;
        text.SetText(slot.text.text);
    }
}
