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
        set
        {
            if (currentSlot == value)
            {
                if(handIsEmpty) EquipItem();
                else UnequipItem();
                return;
            }

            if (!hotbars[value].slot.isEmpty && Time.time - lastSlotChange < 1.5f) return;
            
            hotbars[currentSlot].slot.OnClear.RemoveAllListeners();
            hotbars[currentSlot].slot.OnClear.AddListener(hotbars[currentSlot].OnClear);
            lastSlot = currentSlot;
            currentSlot = value;
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
            Invoke(nameof(EquipItem), 0.9f);
            return;
        }
        
        handIsEmpty = false;

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;

        if (!currentItem)
        {
            UnequipItem();
            return;
        }
        
        hotbars[currentSlot].slot.OnClear.AddListener(UnequipItem);
        
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
                AnimatorEquipTool(true);
                break;
            case ItemTag.Sword:
                AnimatorEquipSword(true);
                break;
            case ItemTag.Spear:
                AnimatorEquipSpear(true);
                break;
            case ItemTag.Bow:
                AnimatorEquipBow(true);
                Invoke(nameof(SetBowAnimator), 0.2f);
                break;
            case ItemTag.Grappling:
                AnimatorEquipGrappling(true);
                break;
            default:
                Debug.Log($"Item Tag Not Implemented {hotbars[currentSlot].slot.inventoryItem.itemTag}");
                break;
        }
    }

    public void ShowModel()
    {
        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;
        switch (currentItem.itemTag)
        {
            case ItemTag.Axe:
            case ItemTag.Pickaxe:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, toolBone);
                break;
            case ItemTag.Sword:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, swordBone);
                break;
            case ItemTag.Bow:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, bowBone);
                break;
            case ItemTag.Spear:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, spearBone);
                break;
            case ItemTag.Grappling:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, grapplingBone);
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
        if(currentWeapon) Destroy(currentWeapon);
    }

    private void SetBowAnimator()
    {
        if(currentWeapon) currentWeapon.TryGetComponent(out bow);
    }
}
