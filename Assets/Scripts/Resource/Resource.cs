using Unity.Netcode;

public class Resource : NetworkBehaviour
{
    public string resourceName;
    public int maxHealth;
    public int maxResources;
    public NetworkVariable<int> curHealth = new();
    public NetworkVariable<int> curResources = new();

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(NetworkBehaviourReference player, string rscName, int damage)
    {
        curHealth.Value -= damage;

        if (curHealth.Value <= 0) DestroyResource();
            
        int resourcesGathered = (int)(maxResources * (damage / (float)maxHealth));
        curResources.Value -= resourcesGathered;
        
        if(player.TryGet(out Player ply))
        {
            ply.GatherResourcesClientRpc(rscName, resourcesGathered);
        }
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}