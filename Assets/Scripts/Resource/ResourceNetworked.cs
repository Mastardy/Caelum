using Unity.Netcode;

public class ResourceNetworked : NetworkBehaviour
{
    public string resourceName;
    public NetworkVariable<DynamicValueNetworked<int>> Health;
    public NetworkVariable<DynamicValueNetworked<int>> ResourceAmount;

    public int HitResource(int damage)
    {
        var health = Health.Value;
        var resourceAmount = ResourceAmount.Value;
        
        health.current -= damage;

        health.current -= damage;
        
        if(health.current <= 0) DestroyResource();
        
        int resourceGathered = (int)(resourceAmount.max * (damage / (float)health.max));
        resourceAmount.current -= resourceGathered;

        Health.Value = health;
        ResourceAmount.Value = resourceAmount;
        
        return resourceGathered;
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}