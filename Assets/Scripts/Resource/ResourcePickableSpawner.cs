using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ResourcePickableSpawner : NetworkBehaviour
{
    public static List<ResourcePickableSpawner> spawners = new();
    public static bool handled;
    
    public Transform[] locations;
    public GameObject fruit;
    [Range(1, 10)] public int chance = 1;

    private void Awake()
    {
        spawners.Add(this);
    }

    private void Start()
    {
        if (handled) return;
        StartPoint.Instance.OnStart.AddListener(TreeStart);
        handled = true;
    }

    private void TreeStart()
    {
        if(IsHost) Invoke("SpawnFruitServerRpc", 1f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnFruitServerRpc()
    {
        if (!IsServer) return;
        
        foreach (var spawner in spawners)
        {
            if (spawner.locations.Length == 0) continue;
            
            for (int i = 0; i < locations.Length; i++)
            {
                int rand = Random.Range(0, 10);
                if (rand <= spawner.chance)
                {
                    var fruitinst = Instantiate(spawner.fruit, spawner.locations[i]);
                    fruitinst.transform.Rotate(Vector3.up, Random.Range(0, 360));
                    float randScale = Random.Range(0.2f, 0.3f);
                    fruitinst.transform.localScale = new Vector3(randScale, randScale, randScale);

                    fruitinst.name = spawner.fruit.name;
                    fruitinst.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
