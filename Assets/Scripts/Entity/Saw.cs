using Unity.Netcode;
using UnityEngine;

public class Saw : NetworkBehaviour
{
    public bool isSawing;
    public float sawTimer;
    
    [ServerRpc(RequireOwnership = false)]
    public void SawStartServerRpc()
    {
        if (!IsServer) return;
        
        isSawing = true;
        sawTimer = Time.time;
    }
}
