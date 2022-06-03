using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;

public class FishingNet : NetworkBehaviour
{
    public int fishesInNet;

    [SerializeField] private Vector2 timeRange = new Vector2(5, 10);
    [SerializeField] private Vector2 extraFishTimeRange = new Vector2(1, 3);

    //model variables
    [SerializeField] private Animator wheelanimator;
    [SerializeField] private Animator fishanimator;
    [SerializeField] private Transform tipAnchor;
    [SerializeField] private GameObject ropeTip;

    private bool netLaunched;
    private float launchTime;
    private float fishingTime;
    private float nextFishTime;
    
    [ServerRpc]
    public void TryFishingServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            if (!netLaunched)
            {
                netLaunched = true;
                AnimatorRelease();
                launchTime = Time.time;
                fishingTime = Random.Range(timeRange.x, timeRange.y);
                nextFishTime = fishingTime;
                
                return;
            }

            if (Time.time - launchTime > fishingTime)
            {
                netLaunched = false;
                AnimatorRetrieve();
                AnimatorCatchFish(false);
                ply.GiveItemServerRpc(player, "raw_fish", fishesInNet);
                fishesInNet = 0;
            }
        }
    }

    private void Update()
    {
        //alterar localização da corda apenas se tiver tocando as animaçoes, e nao em idle
        if(wheelanimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_ReleaseKite") || wheelanimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_RetrieveKite") || wheelanimator.GetCurrentAnimatorStateInfo(0).IsName("Wheel_KiteInAir"))
            ropeTip.transform.position = tipAnchor.position;

        if (!netLaunched) return;
        if (Time.time - launchTime < fishingTime) return;
        if (Time.time - launchTime < nextFishTime) return;
        
        fishesInNet++;
        AnimatorCatchFish(true);
        nextFishTime += 1 / Random.Range(extraFishTimeRange.x, extraFishTimeRange.y);
    }

    private void AnimatorRelease()
    {
        wheelanimator.SetTrigger("Release");
    }

    private void AnimatorRetrieve()
    {
        wheelanimator.SetTrigger("Retrieve");
    }

    private void AnimatorCatchFish(bool c)
    {
        fishanimator.SetBool("Catch", c);
    }
}