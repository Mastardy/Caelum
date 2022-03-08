using Unity.Netcode;

public class Resource : NetworkBehaviour
{
    public string resourceName;
    public int maxHealth;
    public int maxResources;
    public NetworkVariable<int> curHealth = new();
    public NetworkVariable<int> curResources = new();

    private int resourcesGathered;
    
    public int HitResource(int damage)
    {
        HitResourceServerRpc(damage);

        return resourcesGathered;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(int damage)
    {
        curHealth.Value -= damage;

        if (curHealth.Value <= 0) DestroyResource();
            
        resourcesGathered = (int)(maxResources * (damage / (float)maxHealth));
        curResources.Value -= resourcesGathered;
    }
    
    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}