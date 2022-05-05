using Unity.Netcode;
using UnityEngine;

public class Oven : NetworkBehaviour
{
    private bool hasItem;
    private float itemTimer;
    
    [ServerRpc(RequireOwnership = false)]
    public void OpenOvenServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;

        if (player.TryGet(out Player ply))
        {
            ply.OpenOvenClientRpc(this);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CloseOvenServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        
        if (player.TryGet(out Player ply))
        {
            ply.CloseOvenClientRpc();
        }
    }
}
