using UnityEngine;
using System.Collections;
using Unity.Netcode;

public partial class Animal
{
    [HideInInspector] public bool dead;
    public int maxHealth = 150;
    public NetworkVariable<int> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    private static readonly int damagedStrength = Shader.PropertyToID("_Damaged_Strength");

    private MaterialPropertyBlock animalMpb;
    private MaterialPropertyBlock AnimalMpb
    {
        get
        {
            if (animalMpb == null) animalMpb = new MaterialPropertyBlock();
            return animalMpb;
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damageTaken, NetworkBehaviourReference player)
    {
        currentHealth.Value -= damageTaken;
        if (!dead){
            animator.SetTrigger(hitCache);
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

    private IEnumerator FadeColor()
    {
        AnimalMpb.SetFloat(damagedStrength, 1);
        modelRenderer.SetPropertyBlock(AnimalMpb);

        while (true) {
            AnimalMpb.SetFloat(damagedStrength, AnimalMpb.GetFloat(damagedStrength) - Time.deltaTime * 3);
            
            if(AnimalMpb.GetFloat(damagedStrength) < 0) AnimalMpb.SetFloat(damagedStrength, 0);

            modelRenderer.SetPropertyBlock(AnimalMpb);
            
            yield return AnimalMpb.GetFloat(damagedStrength) <= 0;
        }
    }
}
