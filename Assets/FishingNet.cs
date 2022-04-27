using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingNet : NetworkBehaviour
{
    [SerializeField] private Vector2 timeRange = new Vector2(5, 10);
    [SerializeField] private float extraFishPerSecond = 0.25f;

    private float fishingTime;
    private int fishesInNet;
    private float launchTime;
    private bool netLaunched;

    private float nextFishTime;
    
    [ServerRpc]
    public void TryFishingServerRpc(NetworkBehaviourReference player)
    {
        if (player.TryGet(out Player ply))
        {
            if (!netLaunched)
            {
                fishesInNet = 0;
                netLaunched = true;
                launchTime = Time.time;
                fishingTime = Random.Range(timeRange.x, timeRange.y);
                nextFishTime = fishingTime + 1 / extraFishPerSecond;
                
                return;
            }

            if (Time.time - launchTime > fishingTime)
            {
                netLaunched = false;
                ply.GiveItemClientRpc(6, fishesInNet);
            }
        }
    }

    private void Update()
    {
        if (!netLaunched) return;
        if (Time.time - launchTime < fishingTime) return;
        if (Time.time - launchTime < nextFishTime) return;
        
        fishesInNet++;
        nextFishTime += 1 / extraFishPerSecond;
        Debug.Log(fishesInNet);
    }
}
