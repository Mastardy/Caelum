using Unity.Netcode;

public class Resource : NetworkBehaviour
{
    public int resourceId;
    public int maxHealth;
    public int maxResources;
    public NetworkVariable<int> curHealth = new();
    public NetworkVariable<int> curResources = new();

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(NetworkBehaviourReference player)
    {
        curHealth.Value -= 1;

        if (curHealth.Value <= 0) DestroyResource();
            
        int resourcesGathered = (int)(maxResources * (1 / (float)maxHealth));
        curResources.Value -= resourcesGathered;
        
        if(player.TryGet(out Player ply))
        {
            ply.PickUpClientRpc(resourceId);
        }
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}