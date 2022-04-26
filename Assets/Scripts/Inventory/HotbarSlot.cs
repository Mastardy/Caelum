using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] private InventorySlot slot;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    
    private void Update()
    {
        if (slot.isEmpty)
        {
            image.enabled = false;
            image.sprite = null;
            text.SetText(string.Empty);
            return;
        }

        image.enabled = true;
        image.sprite = slot.image.sprite;
        text.SetText(slot.text.text);
    }
}
