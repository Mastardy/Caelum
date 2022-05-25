using UnityEngine;
using Unity.Netcode;

public class FruitSpawner : NetworkBehaviour
{
    public void RemoveFruit()
    {
        var fruits = GetComponentsInChildren<Transform>();
        if (fruits[1])
        {
            RemoveFruitServerRpc(fruits[1].GetComponent<NetworkObject>());
        }
    }

    [ServerRpc]
    private void RemoveFruitServerRpc(NetworkObjectReference fruit)
    {
        if (!IsServer) return;
        
        if(fruit.TryGet(out NetworkObject fruitNetworkObject))
        {
            fruitNetworkObject.Despawn();
            Destroy(fruitNetworkObject.gameObject);
        }
    }
}
