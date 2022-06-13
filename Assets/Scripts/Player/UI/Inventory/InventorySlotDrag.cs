using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotDrag : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Player player;
    
    [SerializeField] private Transform parentTransform;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform textRectTransform;
    [SerializeField] private RectTransform durabilityRectTransform;
    [SerializeField] private InventorySlot inventorySlot;

    private PointerEventData.InputButton mode;

    private GameObject[] tempGameObjects = new GameObject[3];
    
    private Transform startParent;
    private Vector3 startPosition;
    private Vector3 mouseOffset;
    private Vector3 textOffset;
    private Vector3 durabilityOffset;

    private bool hovering;
    private bool busy;

    [SerializeField] private GameObject itemDescriptionPrefab;
    private GameObject itemDescription;

    private InventorySlot selfInvSlot;

    private void Awake()
    {
        selfInvSlot = GetComponent<InventorySlot>();
    }

    private void Update()
    {
        if (!selfInvSlot.inventoryItem) return;   
        
        if (!hovering) return;

        if (!itemDescription)
        {
            itemDescription = Instantiate(itemDescriptionPrefab, transform.position, Quaternion.identity, parentTransform);
            itemDescription.GetComponent<ItemDescription>().title.SetText(inventorySlot.inventoryItem.name); 
            itemDescription.GetComponent<ItemDescription>().description.SetText(inventorySlot.inventoryItem.description);
        }

        ((RectTransform)itemDescription.transform).position = Input.mousePosition + new Vector3(25, -25, 0);
        
        if (Input.GetKeyDown(/*GameManager.Instance.gameOptions.dropKey*/KeyCode.G))
        {
            player.DropItem(inventorySlot, Input.GetKey(KeyCode.LeftShift));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(itemDescription) Destroy(itemDescription);
        hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData) => mouseOffset = rectTransform.position - Input.mousePosition;
        
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!selfInvSlot.inventoryItem) return;
        
        if (busy) return;
        
        busy = true;
        mode = eventData.button;

        var parent = rectTransform.parent;
        var pos = rectTransform.position;
        durabilityOffset = durabilityRectTransform.position - pos;
        textOffset = textRectTransform.position - pos;
        startParent = parent;
        rectTransform.SetParent(parentTransform);
        textRectTransform.SetParent(parentTransform);
        durabilityRectTransform.SetParent(parentTransform);
        startPosition = pos;

        if (mode != PointerEventData.InputButton.Left && inventorySlot.Amount > 1)
        {
            tempGameObjects[0] = Instantiate(gameObject, transform.position, Quaternion.identity, parent);
            tempGameObjects[0].GetComponent<InventorySlotDrag>().enabled = false;
            tempGameObjects[1] = Instantiate(textRectTransform.gameObject, textRectTransform.position, Quaternion.identity, parent);
            tempGameObjects[1].GetComponentsInChildren<TextMeshProUGUI>()[0].SetText(Mathf.CeilToInt(inventorySlot.Amount / 2f).ToString());
            tempGameObjects[2] = Instantiate(durabilityRectTransform.gameObject, durabilityRectTransform.position, Quaternion.identity, parent);
            textRectTransform.GetComponentsInChildren<TextMeshProUGUI>()[0].SetText(Mathf.FloorToInt(inventorySlot.Amount / 2f).ToString());
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!selfInvSlot.inventoryItem) return;
        rectTransform.position = new Vector3(eventData.position.x, eventData.position.y, 0) + mouseOffset;
        var pos = rectTransform.position;
        textRectTransform.position = pos + textOffset;
        durabilityRectTransform.position = pos + durabilityOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!selfInvSlot.inventoryItem) return;
        busy = false;
        
        Destroy(tempGameObjects[0]);
        Destroy(tempGameObjects[1]);
        Destroy(tempGameObjects[2]);
        
        rectTransform.SetParent(startParent);
        textRectTransform.SetParent(startParent);
        durabilityRectTransform.SetParent(startParent);
        
        rectTransform.position = startPosition;
        var pos = rectTransform.position;
        textRectTransform.position = pos + textOffset;
        durabilityRectTransform.position = pos + durabilityOffset;
        
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count == 0)
        {
            player.DropItem(inventorySlot, true);
            return;
        }
        
        foreach (var raycastResult in raycastResults)
        {
            inventorySlot.Amount = inventorySlot.Amount;
            
            if (raycastResult.gameObject.TryGetComponent(out InventorySlot invSlot))
            {
                if (invSlot == inventorySlot) return;

                if (invSlot.isEmpty)
                {
                    if (mode == PointerEventData.InputButton.Left || inventorySlot.Amount == 1)
                    {
                        invSlot.Fill(inventorySlot.inventoryItem, inventorySlot.Amount, inventorySlot.Durability);
                        inventorySlot.Clear();
                    }
                    else
                    {
                        invSlot.Fill(inventorySlot.inventoryItem, Mathf.FloorToInt(inventorySlot.Amount / 2f), inventorySlot.Durability);
                        inventorySlot.Amount = Mathf.CeilToInt(inventorySlot.Amount / 2f);
                    }
                }
                else
                {
                    if (invSlot.inventoryItem == null) return;
                    if (invSlot.inventoryItem.itemName != inventorySlot.inventoryItem.itemName)
                    {
                        var tempInvItem = inventorySlot.inventoryItem;
                        var tempAmount = inventorySlot.Amount;
                        var tempDurability = inventorySlot.Durability;
                        
                        inventorySlot.Clear();
                        inventorySlot.Fill(invSlot.inventoryItem, invSlot.Amount, invSlot.Durability);
                        
                        invSlot.Clear();
                        invSlot.Fill(tempInvItem, tempAmount, tempDurability);
                        return;
                    }

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