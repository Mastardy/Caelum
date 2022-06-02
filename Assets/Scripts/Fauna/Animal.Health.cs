using UnityEngine;
using Unity.Netcode;

public partial class Animal
{
    [HideInInspector] public bool dead;
    public int maxHealth = 150;
    public NetworkVariable<int> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    [ServerRpc]
    public void TakeDamageServerRpc(int damageTaken, NetworkBehaviourReference player)
    {
        currentHealth.Value -= damageTaken;
        if(!dead)
            animator.SetTrigger(hitCache);

        if (currentHealth.Value <= 0)
        {
            if (player.TryGet(out Player ply)) ply.GiveItemClientRpc("raw_meat");
            currentHealth.Value = 0;
            Die();
        }
    }

    private void Die()
    {
        dead = true;
        CurrentState = idleState;
        stateText.SetText("Dead");
        animator.SetBool(deadCache, true);
    }
}
