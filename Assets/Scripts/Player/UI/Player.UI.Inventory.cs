using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    private bool inInventory;
    private Dictionary<string, InventoryItem> inventoryItems = new();
    
    /// <summary>
    /// Hides the Inventory
    /// </summary>
    public void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inInventory = false;
        takeInput = true;
        inventoryPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Opens the Inventory
    /// </summary>
    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inInventory = true;
        takeInput = false;
        inventoryPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
    }
    
    [ClientRpc]
    public void GiveItemClientRpc(string itemName, int amountToAdd = 1)
    {
        int amountAdded = 0;
        
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

            slot.Amount = 0;
            slot.isEmpty = false;
            slot.inventoryItem = inventoryItems[itemName];
            slot.image.sprite = slot.inventoryItem.sprite;
            slot.image.enabled = true;
            
            do
            {
                slot.Amount++;
                amountAdded++;
            } while (slot.Amount < slot.inventoryItem.maxStack && amountAdded < amountToAdd);

            if (amountAdded >= amountToAdd) return;
        }
    }

    public void DropItem(InventorySlot inventorySlot, bool dropEverything = false)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].GetInstanceID() == inventorySlot.GetInstanceID())
            {
                DropItemServerRpc(this, inventorySlot.inventoryItem.itemName, i, inventorySlots[i].Amount, dropEverything);
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(NetworkBehaviourReference ply, string itemName, int slot, int dropAmount, bool dropEverything)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            var playerTransform = player.playerCamera.transform;
            var playerTransformForward = playerTransform.forward;
            
            var worldGameObject = Instantiate(player.inventoryItems[itemName].worldPrefab, playerTransform.position + playerTransformForward,
                player.inventoryItems[itemName].worldPrefab.transform.rotation);
            worldGameObject.name = player.inventoryItems[itemName].name;

            var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = player.inventoryItems[itemName];
            worldGameObjectInvItem.amount = dropEverything ? dropAmount : 1;

            worldGameObject.GetComponent<NetworkObject>().Spawn();
            worldGameObject.GetComponent<Rigidbody>().AddForce(playerTransformForward * 2, ForceMode.Impulse);
            
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
    }
}