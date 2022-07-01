using Unity.Netcode;
using UnityEngine;

public class ResourcePickable : NetworkBehaviour
{
    [SerializeField] private string resourceName;
    public float respawnTimer = 600;
    
    [ServerRpc]
    public void GatherResourceServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            ply.GiveItemServerRpc(player, resourceName);
            gameObject.SetActive(false);
            Invoke("Respawn", respawnTimer);
        }
    }

    private void Respawn()
    {
        gameObject.SetActive(true);
    }
}
