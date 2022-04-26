using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [SerializeField] private int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(int newValue)
    {
        if (!IsServer) return;
        
        currentHealth.Value = newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int value)
    {
        if (!IsServer) return;
        
        currentHealth.Value -= value;

        if (currentHealth.Value > maxHealth) currentHealth.Value = maxHealth;
        if (currentHealth.Value < 1) Debug.Log("Player Dead");
    }
}