using Unity.Netcode;
using UnityEngine;

public class Resource : NetworkBehaviour
{
    public int resourceId;
    public int maxResources;
    public NetworkVariable<int> curResources = new();

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(NetworkBehaviourReference player, int resourcesToGather = 1)
    {
        curResources.Value -= resourcesToGather;

        int resourcesGathered = curResources.Value < 0 ? resourcesToGather + curResources.Value : resourcesToGather;
        
        if (curResources.Value <= 0)
        {
            DestroyResource();
        }
        
        if(player.TryGet(out Player ply))
        {
            ply.GiveItemClientRpc(resourceId, resourcesGathered);
        }
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}