using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceName;
    public DynamicValue<int> Health;
    public DynamicValue<int> ResourceAmount;

    public int HitResource(int damage)
    {
        Health.current -= damage;

        if(Health.current <= 0) DestroyResource();
        
        int resourceGathered = (int)(ResourceAmount.max * (damage / (float)Health.max));
        ResourceAmount.current -= resourceGathered;

        return resourceGathered;
    }

    public void DestroyResource()
    {
        // Tocar Animação
        Destroy(gameObject);
    }
}