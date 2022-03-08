using Unity.Netcode;

public class Resource : NetworkBehaviour
{
    public string resourceName;
    public int maxHealth;
    public int maxResources;
    public NetworkVariable<int> curHealth = new();
    public NetworkVariable<int> curResources = new();

    public int HitResource(int damage)
    {
        HitResourceServerRpc(this, damage, out int resourceGathered);

        return resourceGathered;
    }

    [ServerRpc]
    public void HitResourceServerRpc(NetworkBehaviourReference resource, int damage, out int resourceGathered)
    {
        resourceGathered = 0;
        
        if (resource.TryGet(out Resource resourceComponent))
        {
            resourceComponent.curHealth.Value -= damage;

            if (curHealth.Value <= 0) DestroyResource();
            
            resourceGathered = (int)(maxResources * (damage / (float)maxHealth));
            curResources.Value -= resourceGathered;
        }
    }
    
    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}