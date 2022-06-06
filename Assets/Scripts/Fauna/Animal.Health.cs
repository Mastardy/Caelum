using UnityEngine;
using System.Collections;
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
        if (!dead){
            animator.SetTrigger(hitCache);
            FlashColor();
            Invoke(nameof(FadeColor), 0.1f);
        }

        if (currentHealth.Value <= 0)
        {
            if (player.TryGet(out Player ply)) ply.GiveItemServerRpc(player, "raw_meat");
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
        foreach(var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    private void FlashColor()
    {
        modelRenderer.sharedMaterial.SetFloat("_Damaged_Strength", 1);
    }

    private void ResetColor()
    {
        modelRenderer.sharedMaterial.SetFloat("_Damaged_Strength", 0);
    }

    IEnumerator FadeColor()
    {
        float fade = 1;
        do
        {
            modelRenderer.sharedMaterial.SetFloat("_Damaged_Strength", fade);
            fade -= 0.01f;
        } while (modelRenderer.sharedMaterial.GetFloat("_Damaged_Strength") > 0);

        yield return null;
    }
}
