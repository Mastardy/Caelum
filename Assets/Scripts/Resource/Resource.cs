using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Resource : NetworkBehaviour
{
    [SerializeField] public InventoryItem item;
    public int resourcesQuantity;
    public NetworkVariable<int> curHP = new();

    public UnityEvent onGather;

    //animator stuff
    public Animator resourceAnimator;
    private NetworkVariable<bool> fallNet = new(readPerm: NetworkVariableReadPermission.Everyone); //boleano que faz o animator derrubar a arvore

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(int resourceHP = 1)
    {
        curHP.Value -= resourceHP;
        
        if (curHP.Value <= 0)
        {
            if (item.itemName == "stone")
                DestroyResource();
            else
                DestroyResourceAnimation();
        }
    }

    public void DestroyResource()
    {
        for(int i = 0; i < resourcesQuantity; i++)
        {
            DropResourceServerRpc(transform.position + new Vector3(Random.Range(-2f, 2f), 2, Random.Range(-2f, 2f)));
        }
        Destroy(gameObject);
    }

    public void DestroyResourceAnimation()
    {
        fallNet.Value = true;
        resourceAnimator.SetBool("Fall", fallNet.Value);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DropResourceServerRpc(Vector3 pos)
    {
        var worldGameObject = Instantiate(item.worldPrefab, pos, Quaternion.identity);

        worldGameObject.name = item.itemName;

        var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
        worldGameObjectInvItem.inventoryItem = item;
        worldGameObjectInvItem.amount = 1;
        worldGameObjectInvItem.durability = 1;

        worldGameObject.GetComponent<NetworkObject>().Spawn();
        worldGameObject.GetComponent<Rigidbody>().AddForce(worldGameObject.transform.up + new Vector3(Random.Range(-0.1f, 0.1f), 0.5f, Random.Range(-0.1f, 0.1f)), ForceMode.Impulse);
    }
}