using Unity.Netcode;
using UnityEngine;

public class ResourcePickable : NetworkBehaviour
{
    [SerializeField] private string resourceName;
    
    [ServerRpc]
    public void GatherResourceServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            ply.GiveItemServerRpc(player, resourceName);
            Destroy(gameObject);
        }
    }
}
