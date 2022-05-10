using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrag : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Player player;
    
    [SerializeField] private Transform parentTransform;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private InventorySlot inventorySlot;

    private PointerEventData.InputButton mode;

    private GameObject[] tempGameObjects = new GameObject[2];
    
    private Transform startParent;
    private Vector3 startPosition;
    private Vector3 mouseOffset;
    private Vector3 textOffset;

    private bool hovering;
    private bool busy;
    
    private void Update()
    {
        if (!hovering) return;

        if (Input.GetKeyDown(/*GameManager.Instance.gameOptions.dropKey*/KeyCode.G))
        {
            player.DropItem(inventorySlot, Input.GetKey(KeyCode.LeftShift));
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => hovering = true;

    public void OnPointerExit(PointerEventData eventData) => hovering = false;

    public void OnPointerClick(PointerEventData eventData) => mouseOffset = rectTransform.position - Input.mousePosition;
        
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (busy) return;
        
        busy = true;
        mode = eventData.button;

        var parent = rectTransform.parent;
        textOffset = textRectTransform.position - rectTransform.position;
        startParent = parent;
        rectTransform.SetParent(parentTransform);
        textRectTransform.SetParent(parentTransform);
        startPosition = rectTransform.position;

        if (mode != PointerEventData.InputButton.Left && inventorySlot.Amount > 1)
        {
            tempGameObjects[0] = Instantiate(gameObject, transform.position, Quaternion.identity, parent);
            tempGameObjects[0].GetComponent<InventorySlotDrag>().enabled = false;
            tempGameObjects[1] = Instantiate(textRectTransform.gameObject, textRectTransform.position, Quaternion.identity, parent);
            tempGameObjects[1].GetComponent<TextMeshProUGUI>().SetText(Mathf.CeilToInt(inventorySlot.Amount / 2f) + "x");
            textRectTransform.GetComponent<TextMeshProUGUI>().SetText(Mathf.FloorToInt(inventorySlot.Amount / 2f) + "x");
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = new Vector3(eventData.position.x, eventData.position.y, 0) + mouseOffset;
        textRectTransform.position = rectTransform.position + textOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        busy = false;
        Destroy(tempGameObjects[0]);
        Destroy(tempGameObjects[1]);
        
        rectTransform.SetParent(startParent);
        textRectTransform.SetParent(startParent);
        rectTransform.position = startPosition;
        textRectTransform.position = rectTransform.position + textOffset;

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (var raycastResult in raycastResults)
        {
            if(inventorySlot.Amount != 0) inventorySlot.text.SetText(inventorySlot.Amount + "x");
            
            if (raycastResult.gameObject.TryGetComponent(out InventorySlot invSlot))
            {
                if (invSlot == inventorySlot) return;

                if (invSlot.isEmpty)
                {
                    if (mode == PointerEventData.InputButton.Left || inventorySlot.Amount == 1)
                    {
                        invSlot.Amount = inventorySlot.Amount;
                        inventorySlot.Clear();
                    }
                    else
                    {
                        invSlot.Amount = Mathf.FloorToInt(inventorySlot.Amount / 2f);
                        inventorySlot.Amount = Mathf.CeilToInt(inventorySlot.Amount / 2f);
                    }
                    
                    invSlot.image.sprite = inventorySlot.image.sprite;
                    invSlot.image.enabled = true;
                    invSlot.isEmpty = false;
                    invSlot.inventoryItem = inventorySlot.inventoryItem;
                }
                else
                {
                    if (invSlot.inventoryItem == null) return;
                    if (invSlot.inventoryItem.itemName != inventorySlot.inventoryItem.itemName) return;

                    if (mode == PointerEventData.InputButton.Left || inventorySlot.Amount == 1)
                    {
                        if (invSlot.Amount + inventorySlot.Amount > inventorySlot.inventoryItem.maxStack)
                        {
                            (invSlot.Amount, inventorySlot.Amount) = (inventorySlot.Amount, invSlot.Amount);
                            return;
                        }

                        invSlot.Amount += inventorySlot.Amount;

                        inventorySlot.Clear();
                    }
                    else
                    {
                        if (invSlot.Amount + Mathf.FloorToInt(inventorySlot.Amount / 2f) > inventorySlot.inventoryItem.maxStack) return;
                        
                        invSlot.Amount += Mathf.FloorToInt(inventorySlot.Amount / 2f);
                        inventorySlot.Amount = Mathf.CeilToInt(inventorySlot.Amount / 2f);
                    }
                }
            }
        }
    }
}