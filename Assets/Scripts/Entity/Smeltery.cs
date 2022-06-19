using Unity.Netcode;
using UnityEngine;

public class Smeltery : NetworkBehaviour
{
    public bool isSmelting;
    public float smelteryTimer;
    
    private void Awake()
    {
        isSmelting = false;
        smelteryTimer = 0;
    }
    
    [ServerRpc]
    public void SmeltStartServerRpc()
    {
        if (!IsServer) return;

        isSmelting = true;
        smelteryTimer = Time.time;
        
        Invoke(nameof(SmeltEndServerRpc), 3f);
    }
    
    [ServerRpc]
    public void SmeltEndServerRpc()
    {
        if (!IsServer) return;

        isSmelting = false;
    }
}