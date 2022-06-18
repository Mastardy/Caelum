using Unity.Netcode;
using UnityEngine;

public class InventoryGroundItem : NetworkBehaviour
{
    public InventoryItem inventoryItem;
    [HideInInspector] public NetworkVariable<int> amount = new(readPerm: NetworkVariableReadPermission.Everyone);
    private float durability;
    public float Durability
    {
        get => durability;
        set
        {
            durability = value;
            if(durability <= 0.01 && inventoryItem.itemTag is ItemTag.Axe or ItemTag.Bow or ItemTag.Pickaxe or ItemTag.Spear or ItemTag.Sword or ItemTag.Armor or ItemTag.Grappling) Destroy(gameObject);
        }
    }
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
            player.GiveItemServerRpc(player, inventoryItem.itemName, amount.Value, Durability);
        }
    }

    [ClientRpc]
    public void ChangeNameClientRpc(string itemName)
    {
        name = itemName;
    }
    
    private void Start()
    {
        name = inventoryItem.worldPrefab.name;
        groundItemLayerMask = LayerMask.GetMask("GroundItem");
        InvokeRepeating(nameof(CheckForNearbyItems), Random.Range(0.1f, 1f), 2f);
    }

    private void CheckForNearbyItems()
    {
        if (!IsServer) return;
        
        nearResources = Physics.OverlapSphere(transform.position, 1, groundItemLayerMask);
        
        foreach (var nearResource in nearResources)
            if (nearResource.TryGetComponent(out InventoryGroundItem resource))
            {
                if (resource == this) continue;
                
                if (resource.inventoryItem.itemName == inventoryItem.itemName)
                    if (resource.amount.Value + amount.Value <= inventoryItem.maxStack)
                    {
                        amount.Value += resource.amount.Value;

                        Destroy(resource.gameObject);
                    }
            }
    }
}
