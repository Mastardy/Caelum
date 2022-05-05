using Unity.Netcode;

public class Resource : NetworkBehaviour
{
    public string resourceName;
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
            ply.GiveItemClientRpc(resourceName, resourcesGathered);
        }
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}