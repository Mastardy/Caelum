using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CropField : NetworkBehaviour
{
    [SerializeField] private Transform[] seedLocations;
    [SerializeField] private float timeToGrow = 180f;
    
    public NetworkVariable<bool> harvestable;
    
    private Dictionary<string, CropItem> cropItems = new();
    
    private bool hasSeed;
    private List<GameObject> seeds = new();
    private string currentCrop;

    private void Awake()
    {
        var cropItemsList = Resources.LoadAll<CropItem>("CropItems").ToList();
        
        foreach (var cropItem in cropItemsList)
        {
            cropItems.Add(cropItem.cropItem, cropItem);
        }
    }

    [ServerRpc]
    public void PlantSeedsServerRpc(NetworkBehaviourReference player, string crop)
    {
        if(!IsServer) return;
        if (hasSeed) return;
        
        currentCrop = crop;
        var usedNumbers = new List<int>();
        
        for (int i = 0; i < 6; i++)
        {
            var randomNumber = Random.Range(0, seedLocations.Length);
            
            while(usedNumbers.Contains(randomNumber))
            {
                randomNumber = Random.Range(0, seedLocations.Length);
            }
            
            usedNumbers.Add(randomNumber);

            seeds.Add(Instantiate(cropItems[crop].cropPrefab, seedLocations[randomNumber].position + new Vector3(0.1f, 0, 0.1f), seedLocations[randomNumber].rotation, seedLocations[randomNumber]));
            seeds[i].transform.Rotate(Vector3.up, Random.Range(0, 360));

            seeds[i].GetComponent<Crop>().timeToGrow = timeToGrow;
        }

        if (player.TryGet(out Player ply))
        {
            ply.RemoveItem(crop, 1);
        }
        
        hasSeed = true;
        Invoke(nameof(Harvest), timeToGrow);
    }

    private void Harvest()
    {
        harvestable.Value = true;
    }
    
    [ServerRpc]
    public void HarvestServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        if (!hasSeed) return;

        harvestable.Value = false;
        
        foreach (var seed in seeds)
        {
            Destroy(seed);
        }
        
        seeds.Clear();
        
        hasSeed = false;
        
        if(player.TryGet(out Player ply))
        {
            ply.GiveItemServerRpc(player, currentCrop, 6);
        }
    }
}