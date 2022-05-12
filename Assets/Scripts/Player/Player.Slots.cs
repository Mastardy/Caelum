using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Dictionary<string, WeaponItem> weaponItems = new();
    [SerializeField] private Transform weaponBone;

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

        if (currentItem.itemTag is ItemTag.Axe or ItemTag.Pickaxe or ItemTag.Weapon or ItemTag.RangeWeapon)
        {
            currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, weaponBone);
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
            case ItemTag.Weapon:
                switch (currentItem.itemName)
                {
                    case "sword":
                        AnimatorEquipSword(true);
                        break;
                    case "spear":
                        AnimatorEquipSpear(true);
                        break;
                }
                break;
            case ItemTag.RangeWeapon:
                AnimatorEquipBow(true);
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
            case ItemTag.Weapon:
                switch (item.itemName)
                {
                    case "sword":
                        AnimatorEquipSword(false);
                        break;
                    case "spear":
                        AnimatorEquipSpear(false);
                        break;
                }

                break;
            case ItemTag.RangeWeapon:
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
            case ItemTag.Weapon:
                switch (currentItem.itemName)
                {
                    case "sword":
                        AnimatorEquipSword(false);
                        break;
                    case "spear":
                        AnimatorEquipSpear(false);
                        break;
                }

                break;
            case ItemTag.RangeWeapon:
                AnimatorEquipBow(false);
                break;
        }
    }

    private void UnequipWeapon()
    {
        if(currentWeapon) Destroy(currentWeapon);
    }
}
