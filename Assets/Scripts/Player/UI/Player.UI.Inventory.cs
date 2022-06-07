using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    public bool inInventory;
    private Dictionary<string, InventoryItem> inventoryItems = new();

    [SerializeField] private CanvasGroup hotbarsGroup;

    /// <summary>
    /// Hides the Inventory
    /// </summary>
    public void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inInventory = false;
        takeInput = true;
        inventoryPanel.GetComponent<CanvasGroup>().alpha = 0;
        inventoryPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        inventoryPanel.GetComponent<CanvasGroup>().interactable = false;
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;
    }

    /// <summary>
    /// Opens the Inventory
    /// </summary>
    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inInventory = true;
        takeInput = false;
        inventoryPanel.GetComponent<CanvasGroup>().alpha = 1;
        inventoryPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        inventoryPanel.GetComponent<CanvasGroup>().interactable = true;
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        hotbarsGroup.alpha = 0;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void GiveItemServerRpc(NetworkBehaviourReference player, string itemName, int amountToAdd = 1, float durability = 0)
    {
        if (player.TryGet(out Player ply))
        {
            ply.GiveItemClientRpc(player, itemName, amountToAdd, durability);
        }
    }
    
    [ClientRpc]
    public void GiveItemClientRpc(NetworkBehaviourReference player, string itemName, int amountToAdd = 1, float durability = 0)
    {
        if (player.TryGet(out Player ply))
        {
            if (ply != this) return;
        }
        
        int amountAdded = 0;

        if (inventoryItems[itemName].itemTag is ItemTag.Axe or ItemTag.Bow or ItemTag.Pickaxe or ItemTag.Spear or ItemTag.Sword)
        {
            foreach (var hotbar in hotbars)
            {
                var slot = hotbar.slot;
                if (!slot.isEmpty) continue;

                slot.Fill(inventoryItems[itemName], 1, durability);

                return;
            }
        }
        
        foreach (var slot in inventorySlots)
        {
            if (slot.isEmpty) continue;
            if (slot.inventoryItem.itemName != itemName) continue;
            if (slot.Amount == slot.inventoryItem.maxStack) continue;

            do
            {
                slot.Amount++;
                amountAdded++;
            } while (slot.Amount < slot.inventoryItem.maxStack && amountAdded < amountToAdd);

            if (amountAdded >= amountToAdd) return;
        }
        
        foreach (var slot in inventorySlots)
        {
            if (!slot.isEmpty) continue;

            slot.Fill(inventoryItems[itemName], 0, durability);
            
            do
            {
                slot.Amount++;
                amountAdded++;
            } while (slot.Amount < slot.inventoryItem.maxStack && amountAdded < amountToAdd);

            if (amountAdded >= amountToAdd) return;
        }

        if (amountToAdd - amountAdded > 0)
        {
            DropItemServerRpc(transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(1f, 1.5f), Random.Range(-1f, 1f)), itemName, amountToAdd - amountAdded, durability);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(Vector3 pos, string itemName, int dropAmount = 1, float durability = 0)
    {
        var inventoryItem = inventoryItems[itemName];
        
        for (int i = 0; i < dropAmount; i++)
        {
            var worldGameObject = Instantiate(inventoryItem.worldPrefab, pos, Quaternion.identity);

            worldGameObject.name = inventoryItem.worldPrefab.name;

            var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = inventoryItem;
            worldGameObjectInvItem.amount.Value = 1;
            worldGameObjectInvItem.Durability = durability;

            worldGameObject.GetComponent<NetworkObject>().Spawn();   
        }
    }

    public void DropItem(InventorySlot inventorySlot, bool dropEverything = false)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlot == inventorySlots[i])
            {
                DropItemServerRpc(this, inventorySlot.inventoryItem.itemName, i, inventorySlots[i].Amount, 
                    dropEverything, inventorySlots[i].Durability);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(NetworkBehaviourReference ply, string itemName, int slot, int dropAmount, bool dropEverything, float durability = 0)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            var playerTransform = player.playerCamera.transform;
            var playerTransformForward = playerTransform.forward;
            
            var worldGameObject = Instantiate(player.inventoryItems[itemName].worldPrefab, playerTransform.position + playerTransformForward,
                player.inventoryItems[itemName].worldPrefab.transform.rotation);

            var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = player.inventoryItems[itemName];
            worldGameObjectInvItem.amount.Value = dropEverything ? dropAmount : 1;
            worldGameObjectInvItem.Durability = durability;

            worldGameObject.GetComponent<NetworkObject>().Spawn();
            worldGameObject.GetComponent<Rigidbody>().AddForce(playerTransformForward * 2, ForceMode.Impulse);
            worldGameObject.GetComponent<InventoryGroundItem>().ChangeNameClientRpc(inventoryItems[itemName].worldPrefab.name);
            
            player.DropItemClientRpc(slot, dropEverything);
        }
    }

    [ClientRpc]
    public void DropItemClientRpc(int slot, bool dropEverything)
    {
        inventorySlots[slot].Amount = dropEverything ? 0 : inventorySlots[slot].Amount - 1;

        if (inventorySlots[slot].Amount > 0) return;

        inventorySlots[slot].Clear();
    }
    
    public bool CanPickUpItem()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.isEmpty) return true;
        }

        return false;
    }

    private void GetItemsAndRecipes()
    {
        InventoryItem[] invItems = Resources.LoadAll<InventoryItem>("InventoryItems");
        FoodItem[] allFoodItems = Resources.LoadAll<FoodItem>("FoodItems");
        WeaponItem[] allWeaponItems = Resources.LoadAll<WeaponItem>("Weapons");
        cookingRecipes = Resources.LoadAll<CookingRecipe>("CookingRecipes");
        CraftingRecipe[] craftingRecipes = Resources.LoadAll<CraftingRecipe>("CraftingRecipes");

        foreach (var invItem in invItems)
        {
            inventoryItems.Add(invItem.itemName, invItem);
        }

        foreach (var foodItem in allFoodItems)
        {
            foodItems.Add(foodItem.itemName, foodItem);
        }

        foreach (var weaponItem in allWeaponItems)
        {
            weaponItems.Add(weaponItem.itemName, weaponItem);
        }
    }
}
