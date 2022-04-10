using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryGroundItem : NetworkBehaviour
{
    public InventoryItem inventoryItem;
    
    [ServerRpc(RequireOwnership = false)]
    public void PickUpServerRpc(NetworkBehaviourReference ply)
    {
        if (!IsServer) return;
        if (ply.TryGet(out Player player))
        {
            if (!player.CanPickUpItem()) return;
            
            Destroy(gameObject);
            player.PickUpClientRpc(inventoryItem.id);
        }
    }
}
