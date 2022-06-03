using Unity.Netcode;
using UnityEngine;

public class FishingNet : NetworkBehaviour
{
    public NetworkVariable<int> fishesInNet;

    [SerializeField] private Vector2 timeRange = new Vector2(5, 10);
    [SerializeField] private Vector2 extraFishTimeRange = new Vector2(1, 3);

    //model variables
    [SerializeField] private Animator wheelAnimator;
    [SerializeField] private Animator kiteAnimator;
    [SerializeField] private SkinnedMeshRenderer kiteRenderer;
    [SerializeField] private Transform tipAnchor;
    [SerializeField] private GameObject ropeTip;

    private bool netLaunched;
    private float launchTime;
    private float fishingTime;
    private float nextFishTime;
    
    [ServerRpc(RequireOwnership = false)]
    public void TryFishingServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            if (!netLaunched)
            {
                netLaunched = true;
                AnimatorReleaseClientRpc();
                launchTime = Time.time;
                fishingTime = Random.Range(timeRange.x, timeRange.y);
                nextFishTime = fishingTime;
                
                return;
            }

            if (Time.time - launchTime > fishingTime)
            {
                netLaunched = false;
                AnimatorRetrieveClientRpc();
                AnimatorCatchFishClientRpc(false);
                ply.GiveItemServerRpc(player, "raw_fish", fishesInNet.Value);
                fishesInNet.Value = 0;
            }
        }
    }

    private void Update()
    {
        if (wheelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_ReleaseKite") || wheelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_RetrieveKite") || wheelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_KiteInAir"))
        {
            ropeTip.transform.position = tipAnchor.position;
        }

        if (!netLaunched) return;
        if (Time.time - launchTime < fishingTime) return;
        if (Time.time - launchTime < nextFishTime) return;
        
        fishesInNet.Value++;
        AnimatorCatchFishClientRpc(true);
        nextFishTime += 1 / Random.Range(extraFishTimeRange.x, extraFishTimeRange.y);
    }

    [ClientRpc]
    private void AnimatorReleaseClientRpc()
    {
        wheelAnimator.SetTrigger("Release");

        kiteRenderer.sharedMaterial.SetFloat("_Strength", 0.5f);
    }

    [ClientRpc]
    private void AnimatorRetrieveClientRpc()
    {
        wheelAnimator.SetTrigger("Retrieve");
        kiteRenderer.sharedMaterial.SetFloat("_Strength", 0);
    }

    [ClientRpc]
    private void AnimatorCatchFishClientRpc(bool c)
    {
        kiteAnimator.SetBool("Catch", c);
    }
}