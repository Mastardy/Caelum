using Unity.Netcode;

public class CraftingTable : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void OpenCraftingServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        
        if (player.TryGet(out Player ply))
        {
            ply.OpenCraftingClientRpc(this);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CloseCraftingServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        
        if (player.TryGet(out Player ply))
        {
            ply.CloseCraftingClientRpc();
        }
    }
}
