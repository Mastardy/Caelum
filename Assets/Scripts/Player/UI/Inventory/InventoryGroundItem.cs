using Unity.Netcode;
using UnityEngine;

public class InventoryGroundItem : NetworkBehaviour
{
    public InventoryItem inventoryItem;
    [HideInInspector] public int amount = 1;
    [HideInInspector] public float durability;
    [HideInInspector] public Collider[] nearResources;
    [HideInInspector] public LayerMask groundItemLayerMask;
    
    [ServerRpc(RequireOwnership = false)]
    public void PickUpServerRpc(NetworkBehaviourReference ply)
    {
        if (!IsServer) return;
        if (ply.TryGet(out Player player))
        {
            if (!player.CanPickUpItem()) return;
            
            Destroy(gameObject);
            player.GiveItemServerRpc(player, inventoryItem.itemName, amount, durability);
        }
    }

    private void Start()
    {
        groundItemLayerMask = LayerMask.GetMask("GroundItem");
        InvokeRepeating(nameof(CheckForNearbyItems), 1f, 2f);
    }

    private void CheckForNearbyItems()
    {
        nearResources = Physics.OverlapSphere(transform.position, 1, groundItemLayerMask);
        
        foreach (var nearResource in nearResources)
            if (nearResource.TryGetComponent(out InventoryGroundItem resource))
            {
                if (resource == this) continue;
                
                if (resource.inventoryItem.itemName == inventoryItem.itemName)
                    if (resource.amount + amount <= inventoryItem.maxStack)
                    {
                        amount += resource.amount;

                        Destroy(resource.gameObject);
                    }
            }
    }
}
