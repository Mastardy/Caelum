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
    private int lastSlot;
    public int currentSlot;
    private int CurrentSlot
    {
        get => currentSlot;
        set
        {
            if (inParachute) return;
            if (Time.time - lastSlotChange < 0.1f) return;
            var val = value < 0 ? 5 : value > 5 ? 0 : value;
            
            lastSlot = currentSlot;
            currentSlot = val;

            hotbars[lastSlot].Selected = false;
            hotbars[currentSlot].Selected = true;
            
            hotbars[lastSlot].slot.OnClear.RemoveAllListeners();
            hotbars[lastSlot].slot.OnFill.RemoveAllListeners();
            
            hotbars[lastSlot].slot.OnClear.AddListener(hotbars[lastSlot].OnClear);
            hotbars[lastSlot].slot.OnFill.AddListener(hotbars[lastSlot].OnFill);
            
            hotbars[currentSlot].slot.OnFill.AddListener(() => EquipItem());
            hotbars[currentSlot].slot.OnClear.AddListener(() => UnequipItem());

            UnequipItem();
            Invoke("EquipItem", 0.1f);
            
            
            lastSlotChange = Time.time;
        }
    }
    
    private void EquipItem()
    {
        DestroyWeapon();

        InventoryItem currentItem = hotbars[currentSlot].slot.inventoryItem;
        
        switch (currentItem.itemTag)
        {
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
        }
    }

    private void UnequipItem()
    {
        DestroyWeapon();
        
        AnimatorEquipTool(false);
        AnimatorEquipSword(false);
        AnimatorEquipSpear(false);
        AnimatorEquipBow(false);
        AnimatorEquipGrappling(false);
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
