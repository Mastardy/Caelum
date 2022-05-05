using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Status")] 
    [SerializeField] private float hungerTickRate = 0.5f;  
    [SerializeField] private int maxHunger = 250;
    private float lastHungerTick;
    private int currentHunger;
    
    [SerializeField] private float thirstTickRate = 0.2f;
    [SerializeField] private int maxThirst = 100;
    private float lastThirstTick;
    private int currentThirst;

    [SerializeField] private float starvingTickRate = 0.2f;
    private float lastStarvingTick;
    
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

    private void StatusUpdate()
    {
        if (currentHunger > 0)
        {
            if (Time.time - lastHungerTick > 1 / hungerTickRate)
            {
                currentHunger--;
                lastHungerTick = Time.time;
            }
        }
        else
        {
            if (Time.time - lastStarvingTick > 1 / starvingTickRate)
            {
                TakeDamageServerRpc(1);
                lastStarvingTick = Time.time;
            }
        }

        if (currentThirst > 0)
        {
            if (Time.time - lastThirstTick > 1 / thirstTickRate)
            {
                currentThirst--;
                lastThirstTick = Time.time;
            }   
        }
        else
        {
            if (Time.time - lastStarvingTick > 1 / starvingTickRate)
            {
                TakeDamageServerRpc(1);
                lastStarvingTick = Time.time;
            }
        }
    }
}