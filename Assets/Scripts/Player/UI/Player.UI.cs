using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public partial class Player
{
    [Header("UI")]
    [SerializeField] private Canvas playerCanvas;

    [SerializeField] private Transform chatPanel;
    [SerializeField] private GameObject chatEntryPrefab;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject ovenPanel;

    [SerializeField] private GameObject crosshair;
    [SerializeField] private TextMeshProUGUI aimText;
    [SerializeField] private LayerMask hitMask;

    [SerializeField] private GameObject chatBox;
    
    [HideInInspector] public GameObject lookingAt;

    private bool takeInput = true;
    
    [Header("Inventory")]
    [SerializeField] private InventorySlot[] inventorySlots;

    [Header("Oven")]
    [SerializeField] private float ovenSpeed = 2f;
    [SerializeField] private RectTransform ovenArrow;
    [SerializeField] private RectTransform ovenScaler;
}
