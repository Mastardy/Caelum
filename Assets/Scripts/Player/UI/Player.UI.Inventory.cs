using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    private bool inInventory;
    private Dictionary<int, InventoryItem> inventoryItems = new();

    private void InventoryStart()
    {
        GetInventoryItems();
        HideInventory();
    }
    
    /// <summary>
    /// Hides the Inventory
    /// </summary>
    public void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inInventory = false;
        takeInput = true;
        inventory.SetActive(false);
    }

    /// <summary>
    /// Opens the Inventory
    /// </summary>
    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inInventory = true;
        takeInput = false;
        inventory.SetActive(true);
    }
    
    [ClientRpc]
    public void PickUpClientRpc(int id)
    {
        Debug.Log(IsServer);
        foreach (var slot in inventorySlots)
        {
            if (slot.isEmpty) continue;
            Debug.Log(slot.isEmpty);
            if (slot.inventoryItem.id != id) continue;
            if (slot.Amount == slot.inventoryItem.maxStack) continue;
                
            slot.Amount++;
            return;
        }

        foreach (var slot in inventorySlots)
        {
            if (!slot.isEmpty) continue;

            slot.Amount = 1;
            slot.isEmpty = false;
            slot.inventoryItem = inventoryItems[id];
            slot.image.sprite = slot.inventoryItem.sprite;
            slot.image.enabled = true;
            return;
        }
    }

    public void DropItem(int slot)
    {
        if (inventorySlots[slot].isEmpty) return;
        
        DropItemServerRpc(this, inventorySlots[slot].inventoryItem.id, slot);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DropItemServerRpc(NetworkBehaviourReference ply, int item, int slot)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            var worldGameObject = Instantiate(inventoryItems[item].worldPrefab, player.playerCamera.transform.position + player.playerCamera.transform.forward,
                inventoryItems[item].worldPrefab.transform.rotation);
            worldGameObject.GetComponent<NetworkObject>().Spawn();

            worldGameObject.GetComponent<Rigidbody>().AddForce(player.playerCamera.transform.forward * 2, ForceMode.Impulse);
            
            player.DropItemClientRpc(slot);
        }
    }

    [ClientRpc]
    public void DropItemClientRpc(int slot)
    {
        inventorySlots[slot].Amount--;

        if (inventorySlots[slot].Amount > 0) return;
        
        inventorySlots[slot].isEmpty = true;
        inventorySlots[slot].inventoryItem = null;
        inventorySlots[slot].image.sprite = null;
        inventorySlots[slot].image.enabled = false;
    }
    
    public bool CanPickUpItem()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.isEmpty) return true;
        }

        return false;
    }
    
    private void GetInventoryItems()
    {
        InventoryItem[] invItems = Resources.LoadAll<InventoryItem>("InventoryItems");

        foreach (var invItem in invItems)
        {
            inventoryItems.Add(invItem.id, invItem);
        }
    }
}
