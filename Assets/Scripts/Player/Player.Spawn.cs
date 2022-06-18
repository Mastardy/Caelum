using Unity.Mathematics;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Player
{
    [Header("Spawning")]
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float safePositionTimer = 2;
    [SerializeField] private Vector3 safePosition;

    private float respawnTime;
    private float lastSafePosition;

    private void CalculateSafePosition()
    {
        var sideSize = characterController.radius * transform.localScale.x;
        
        if (Physics.CheckBox(groundCheck.position, new Vector3(sideSize, groundDistance, sideSize), transform.rotation, waterMask)) return;
        
        if (isGrounded) safePosition = groundCheck.position + Vector3.up * 0.2f;

        lastSafePosition = Time.time;
    }
    
    private void SpawnPlayer()
    {
        transform.position = spawnPosition;
    }

    [ServerRpc]
    private void DropAllItemsServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            int slotIndex = -1;
            foreach (var invSlot in ply.inventorySlots)
            {
                slotIndex++;
                if (invSlot.isEmpty) continue;
                
                var worldGameObject = Instantiate(invSlot.inventoryItem.worldPrefab, 
                    safePosition + Random.insideUnitSphere + Vector3.up * 2,
                    invSlot.inventoryItem.worldPrefab.transform.rotation);
                
                worldGameObject.name = invSlot.inventoryItem.name;

                var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
                worldGameObjectInvItem.inventoryItem = invSlot.inventoryItem;
                worldGameObjectInvItem.amount.Value = invSlot.Amount;
                worldGameObjectInvItem.Durability = invSlot.Durability;

                worldGameObject.GetComponent<NetworkObject>().Spawn();

                ply.DropItemClientRpc(slotIndex, true);
            }
        }
    }
    
    private void RespawnPlayer(Vector3 position, bool dropItems = true)
    {
        if(dropItems) DropAllItemsServerRpc(this);

        // Advanced Movement
        if(dashing) EndDash();
        if(isTethered) EndGrapple();
        if(isTetheredPlus) EndGrapplePlus();
        
        // Basic Movement
        horizontalVelocity = Vector2.zero;
        verticalVelocity = -1f;

        // Status
        if (dropItems)
        {
            currentHealth.Value = maxHealth;
            currentHunger = maxHunger;
            currentThirst = maxThirst;
        }
        
        respawnTime = Time.time;
        Debug.Log(transform.position);
        transform.position = position;
        Debug.Log(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(safePosition + Vector3.up * 2, 2);
    }
}
