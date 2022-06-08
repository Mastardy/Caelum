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
    public GameObject currentArrow;
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
            if (val == currentSlot) return;
            
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

        if (hotbars[currentSlot].slot.isEmpty) return;

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
                currentWeapon.GetComponent<Bow>().player = this;
                SetBowArrow();
                AnimatorEquipBow(true);
                Invoke(nameof(SetBowAnimator), 0.2f);
                break;
            case ItemTag.Grappling:
                currentWeapon = Instantiate(weaponItems[currentItem.itemName].weaponPrefab, grapplingBone);
                AnimatorEquipGrappling(true);
                break;
        }

        if (currentWeapon)
        {
            currentWeapon.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            currentWeapon.layer = LayerMask.NameToLayer("Weapon");
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

        SetLeftArmWeight(0);
        SetRightArmWeight(0);
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

    #region Bow

    private void SetBowArrow()
    {
        DestroyChildren(currentWeapon.GetComponent<Bow>().arrowAnchor);
        currentArrow = null;
        var arrow = GetPriorityArrow();
        currentWeapon.GetComponent<Bow>().currentArrow = arrow;
        if (arrow == string.Empty) return;
        currentArrow = Instantiate(weaponItems[arrow].weaponPrefab, currentWeapon.GetComponent<Bow>().arrowAnchor);
        currentArrow.layer = LayerMask.NameToLayer("Weapon");
    }
    
    private string GetPriorityArrow()
    {
        if (GetItemAmount("arrow_iron") > 0) return "arrow_iron";
        if (GetItemAmount("arrow_stone") > 0) return "arrow_stone";
        if (GetItemAmount("arrow_wood") > 0) return "arrow_wood";        
        return string.Empty;
    }
    
    private void SetBowAnimator()
    {
        if(currentWeapon) currentWeapon.TryGetComponent(out bow);
    }
    
    #endregion Bow
}
