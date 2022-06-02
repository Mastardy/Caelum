using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Dictionary<string, WeaponItem> weaponItems = new();
    [SerializeField] private Transform toolBone;
    [SerializeField] private Transform spearBone;
    [SerializeField] private Transform bowBone;
    [SerializeField] private Transform swordBone;
    [SerializeField] private Transform grapplingBone;

    public GameObject currentWeapon;
    private Bow bow;
    private float lastSlotChange;
    private bool handIsEmpty = true;
    private int lastSlot;
    public int currentSlot;
    private int CurrentSlot
    {
        get
        {
            return currentSlot;
        }
        set
        {
            if (currentSlot == value)
            {
                if(handIsEmpty) EquipItem();
                else UnequipItem();
                return;
            }

            if (Time.time - lastSlotChange < 0.35f) return;
            var val = value < 0 ? 5 : value > 5 ? 0 : value;
            if (!hotbars[val].slot.isEmpty) return;
            
            hotbars[currentSlot].slot.OnClear.RemoveAllListeners();
            hotbars[currentSlot].slot.OnClear.AddListener(hotbars[currentSlot].OnClear);
            lastSlot = currentSlot;
            currentSlot = val;
            InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;
            hotbars[currentSlot].slot.OnClear.AddListener(() => UnequipItem(currentItem));

            EquipItem();
            lastSlotChange = Time.time;
        }
    }
    
    private void EquipItem()
    {
        hotbars[currentSlot].Selected = true;
        
        if (!handIsEmpty)
        {
            UnequipItem(hotbars[lastSlot].slot.inventoryItem);
            if (hotbars[currentSlot].slot.isEmpty) hotbars[currentSlot].Selected = false;
            Invoke(nameof(EquipItem), 0.2f);
            return;
        }
        
        handIsEmpty = false;

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;

        if (!currentItem)
        {
            DestroyWeapon();
            UnequipItem();
            return;
        }
        
        hotbars[currentSlot].slot.OnClear.AddListener(UnequipItem);
        DestroyWeapon();
        switch (currentItem.itemTag)
        {
            case ItemTag.Other:
                UnequipItem();
                break;
            case ItemTag.Food:
                CurrentSlot = lastSlot;
                break;
            case ItemTag.Axe:
            case ItemTag.Pickaxe:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, toolBone);
                AnimatorEquipTool(true);
                break;
            case ItemTag.Sword:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, swordBone);
                AnimatorEquipSword(true);
                break;
            case ItemTag.Spear:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, spearBone);
                AnimatorEquipSpear(true);
                break;
            case ItemTag.Bow:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, bowBone);
                AnimatorEquipBow(true);
                Invoke(nameof(SetBowAnimator), 0.2f);
                break;
            case ItemTag.Grappling:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, grapplingBone);
                AnimatorEquipGrappling(true);
                break;
            default:
                Debug.Log($"Item Tag Not Implemented {hotbars[currentSlot].slot.inventoryItem.itemTag}");
                break;
        }
    }

    private void UnequipItem(InventoryItem item)
    {
        hotbars[lastSlot].Selected = false;
        
        handIsEmpty = true;

        switch (item.itemTag)
        {
            case ItemTag.Axe:
                AnimatorEquipTool(false);
                break;
            case ItemTag.Pickaxe:
                AnimatorEquipTool(false);
                break;
            case ItemTag.Sword:
                AnimatorEquipSword(false);
                break;
            case ItemTag.Spear:
                AnimatorEquipSpear(false);
                break;
            case ItemTag.Bow:
                bow = null;
                AnimatorEquipBow(false);
                break;
            case ItemTag.Grappling:
                AnimatorEquipGrappling(false);
                break;
        }
    }

    private void UnequipItem()
    {
        DestroyWeapon();

        hotbars[currentSlot].Selected = false;
        
        handIsEmpty = true;

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;
        InventoryItem lastItem = hotbars[currentSlot].slot.inventoryItem;

        if (!currentItem && !lastItem) return;

        switch (currentItem ? currentItem.itemTag : lastItem.itemTag)
        {
            case ItemTag.Axe:
                AnimatorEquipTool(false);
                break;
            case ItemTag.Pickaxe:
                AnimatorEquipTool(false);
                break;
            case ItemTag.Sword:
                AnimatorEquipSword(false);
                break;
            case ItemTag.Spear:
                AnimatorEquipSpear(false);
                break;
            case ItemTag.Bow:
                bow = null;
                AnimatorEquipBow(false);
                break;
            case ItemTag.Grappling:
                AnimatorEquipGrappling(false);
                break;
        }
    }

    public void DestroyWeapon()
    {
        DestroyChildren(toolBone);
        DestroyChildren(swordBone);
        DestroyChildren(bowBone);
        DestroyChildren(grapplingBone);
        DestroyChildren(spearBone);
    }

    private void DestroyChildren(Transform parent)
    {
        var children = parent.GetComponentsInChildren<Transform>();
        if (children.Length <= 1) return;
        for (int i = 1; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
    }

    private void SetBowAnimator()
    {
        if(currentWeapon) currentWeapon.TryGetComponent(out bow);
    }
}
