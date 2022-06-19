using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Status")] 
    [SerializeField] private bool godmode;
    
    [SerializeField] private float hungerTickRate = 0.5f;  
    [SerializeField] private int maxHunger = 250;
    private float lastHungerTick;
    private float currentHunger;
    
    [SerializeField] private float thirstTickRate = 0.2f;
    [SerializeField] private int maxThirst = 100;
    private float lastThirstTick;
    private float currentThirst;

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
        if (godmode) return;
        int lastHealth = currentHealth.Value;
        currentHealth.Value -= value;
        if (lastHealth > currentHealth.Value)
            damageFilter.lastAttack = Time.time;

        if (currentHealth.Value > maxHealth) currentHealth.Value = maxHealth;
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

    public void EatOrDrink(int slot)
    {
        EatOrDrink(inventorySlots[slot]);
    }

    public void EatOrDrink(InventorySlot invSlot)
    {
        var item = invSlot.inventoryItem;

        if (!item) return;
        if (item.itemTag != ItemTag.Food) return;
        
        if (maxHunger * 0.95f < currentHunger && maxThirst * 0.95f < currentThirst) return;
        
        currentHunger += foodItems[item.itemName].hunger;
        if (currentHunger > maxHunger) currentHunger = maxHunger;
        currentThirst += foodItems[item.itemName].thirst;
        if (currentThirst > maxThirst) currentHunger = maxHunger;

        if(invSlot.Amount > 1) invSlot.Amount--;
        else invSlot.Clear();

        AnimatorEat(item.subTag);
    }
}