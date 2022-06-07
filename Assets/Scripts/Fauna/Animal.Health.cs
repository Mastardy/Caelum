using UnityEngine;
using System.Collections;
using Unity.Netcode;

public partial class Animal
{
    [HideInInspector] public bool dead;
    public int maxHealth = 150;
    public NetworkVariable<int> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    private static readonly int damagedStrength = Shader.PropertyToID("_Damaged_Strength");

    [ServerRpc]
    public void TakeDamageServerRpc(int damageTaken, NetworkBehaviourReference player)
    {
        currentHealth.Value -= damageTaken;
        if (!dead){
            animator.SetTrigger(hitCache);
            FlashColor();
            StartCoroutine(FadeColor());
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
            if(!col.isTrigger) col.enabled = false;
        }
    }

    private void FlashColor()
    {
        modelRenderer.sharedMaterial.SetFloat(damagedStrength, 1);
    }

    private IEnumerator FadeColor()
    {
        modelRenderer.sharedMaterial.SetFloat(damagedStrength, 1);

        while (true) {
            modelRenderer.sharedMaterial.SetFloat(damagedStrength, modelRenderer.sharedMaterial.GetFloat(damagedStrength) - Time.deltaTime * 2);

            if(modelRenderer.sharedMaterial.GetFloat(damagedStrength) < 0) modelRenderer.sharedMaterial.SetFloat(damagedStrength, 0);
            yield return modelRenderer.sharedMaterial.GetFloat(damagedStrength) <= 0;
        }
    }
}
