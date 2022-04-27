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
                launchTime = Time.time;
                fishingTime = Random.Range(timeRange.x, timeRange.y);
                nextFishTime = fishingTime;
                
                return;
            }

            if (Time.time - launchTime > fishingTime)
            {
                netLaunched = false;
                ply.GiveItemClientRpc(5, fishesInNet);
                fishesInNet = 0;
            }
        }
    }

    private void Update()
    {
        if (!netLaunched) return;
        if (Time.time - launchTime < fishingTime) return;
        if (Time.time - launchTime < nextFishTime) return;
        
        fishesInNet++;
        nextFishTime += 1 / Random.Range(extraFishTimeRange.x, extraFishTimeRange.y);
    }
}