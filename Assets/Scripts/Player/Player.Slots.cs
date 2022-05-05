using System;
using UnityEngine;

public partial class Player
{
    private bool handIsEmpty;
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

            lastSlot = currentSlot;
            currentSlot = value;
            
            EquipItem();
        }
    }
    
    private void UnequipItem()
    {
        handIsEmpty = true;
        
        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;

        if (currentItem == null)
        {
            currentItem = hotbars[lastSlot].slot.inventoryItem;

            if (currentItem == null) return;

            switch (currentItem.itemTag)
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
            
            return;
        }
        
        switch (currentItem.itemTag)
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
    
    private void EquipItem()
    {
        handIsEmpty = false;

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;

        if (currentItem == null)
        {
            UnequipItem();
            return;
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
}
