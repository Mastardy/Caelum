using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

public partial class Player
{
    [SerializeField] private float maxHealth = 100f;
    public NetworkVariable<float> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    [ServerRpc(RequireOwnership = false)]
    public void SetHealthServerRpc(float newValue)
    {
        if (!IsServer) return;
        
        currentHealth.Value = newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float value)
    {
        if (!IsServer) return;
        
        currentHealth.Value -= value;

        if (currentHealth.Value > maxHealth) currentHealth.Value = maxHealth;
        if (currentHealth.Value < 1) Debug.Log("Player Dead");
    }
}