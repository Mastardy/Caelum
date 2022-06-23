using UnityEngine;
using Unity.Netcode;

public class ResourcePickableSpawner : NetworkBehaviour
{
    public Transform[] locations;
    public GameObject fruit;
    [Range(1, 10)] public int chance = 1;

    private void Start()
    {
        if (locations.Length == 0) return;
        SpawnFruitServerRpc();
    }

    [ServerRpc]
    public void SpawnFruitServerRpc()
    {
        Debug.Log("3");
        if (!IsServer) return;

        for (int i = 0; i < locations.Length; i++)
        {
            int rand = Random.Range(0, 10);
            Debug.Log(rand);
            if (rand <= chance)
            {
                Debug.Log("fruit spawn");
                var fruitinst = Instantiate(fruit, locations[i]);
                fruitinst.transform.Rotate(Vector3.up, Random.Range(0, 360));
                float randScale = Random.Range(0.9f, 1.1f);
                fruitinst.transform.localScale = new Vector3(randScale, randScale, randScale);
            }
        }
    }
}
