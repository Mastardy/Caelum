using TMPro;
using UnityEngine;
using System.Collections.Generic;

public partial class Player
{
    [Header("UI")]
    [SerializeField] private Canvas playerCanvas;

    [SerializeField] private Transform chatPanel;
    [SerializeField] private GameObject chatEntryPrefab;

    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject crafting;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private TextMeshProUGUI aimText;
    [SerializeField] private LayerMask hitMask;

    [SerializeField] private GameObject chatBox;
    
    [HideInInspector] public GameObject lookingAt;

    private bool takeInput = true;
    
    [Header("Inventory")]
    [SerializeField] private InventorySlot[] inventorySlots;
}
