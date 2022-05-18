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

    private GameObject currentWeapon;
    private bool handIsEmpty = true;
    private int lastSlot;
    private int currentSlot;
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
            
            hotbars[currentSlot].slot.OnClear.RemoveAllListeners();
            hotbars[currentSlot].slot.OnClear.AddListener(hotbars[currentSlot].OnClear);
            lastSlot = currentSlot;
            currentSlot = value;
            InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;
            hotbars[currentSlot].slot.OnClear.AddListener(() => UnequipItem(currentItem));
            
            EquipItem();
        }
    }
    
    private void EquipItem()
    {
        if (!handIsEmpty)
        {
            UnequipItem(hotbars[lastSlot].slot.inventoryItem);
            Invoke(nameof(EquipItem), 0.3f);
            return;
        }
        
        handIsEmpty = false;

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;

        if (currentItem == null)
        {
            UnequipItem();
            return;
        }

        hotbars[currentSlot].slot.OnClear.AddListener(UnequipItem);

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
        }
        
        switch (currentItem.itemTag)
        {
            case ItemTag.Other:
                UnequipItem();
                break;
            case ItemTag.Food:
                CurrentSlot = lastSlot;
                break;
            case ItemTag.Axe:
                AnimatorEquipTool(true);
                break;
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
                bowAnimator = currentWeapon.GetComponent<Animator>();
                break;
            default:
                Debug.Log($"Item Tag Not Implemented {hotbars[currentSlot].slot.inventoryItem.itemTag}");
                break;
        }
    }

    private void UnequipItem(InventoryItem item)
    {
        handIsEmpty = true;
        
        Invoke(nameof(UnequipWeapon), 0.3f);
        
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
                bowAnimator = null;
                AnimatorEquipBow(false);
                break;
        }
    }

    private void UnequipItem()
    {
        handIsEmpty = true;
        
        Invoke(nameof(UnequipWeapon), 0.3f);

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
                bowAnimator = null;
                AnimatorEquipBow(false);
                break;
        }
    }

    private void UnequipWeapon()
    {
        if(currentWeapon) Destroy(currentWeapon);
    }
}
