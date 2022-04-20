using Unity.Netcode;
using UnityEngine;

public class InventoryGroundItem : NetworkBehaviour
{
    public InventoryItem inventoryItem;
    [HideInInspector] public int amount = 1;
    public Collider[] nearResources;
    public LayerMask groundItemLayerMask;
    
    [ServerRpc(RequireOwnership = false)]
    public void PickUpServerRpc(NetworkBehaviourReference ply)
    {
        if (!IsServer) return;
        if (ply.TryGet(out Player player))
        {
            if (!player.CanPickUpItem()) return;
            
            Destroy(gameObject);
            player.PickUpClientRpc(inventoryItem.id, amount);
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
                
                if (resource.inventoryItem.id == inventoryItem.id)
                    if (resource.amount + amount <= inventoryItem.maxStack)
                    {
                        amount += resource.amount;

                        Destroy(resource.gameObject);
                    }
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}
