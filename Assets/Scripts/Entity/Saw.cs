using Unity.Netcode;
using UnityEngine;

public class Saw : NetworkBehaviour
{
    private Animator animator;
    
    public bool isSawing;
    public float sawTimer;
    
    private static readonly int animatorIsSawingCache = Animator.StringToHash("isSawing");

    private void Awake()
    {
        isSawing = false;
        sawTimer = 0;
        animator = GetComponent<Animator>();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SawStartServerRpc()
    {
        if (!IsServer) return;
        
        isSawing = true;
        sawTimer = Time.time;

        SawAnimateClientRpc(true);
        
        Invoke(nameof(SawStopServerRpc), 5f);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SawStopServerRpc()
    {
        if (!IsServer) return;

        SawAnimateClientRpc(false);
        
        isSawing = false;
    }

    [ClientRpc]
    private void SawAnimateClientRpc(bool flag)
    {
        animator.SetBool(animatorIsSawingCache, flag);
    }
}