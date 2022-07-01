using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Resource : NetworkBehaviour
{
    [SerializeField] public InventoryItem item;
    public int resourcesQuantity;
    public NetworkVariable<int> curHP = new();
    private int maxHp;
    [Tooltip("Respawn timer in seconds")]
    public float respawnTimer = 300;

    public UnityEvent onGather;

    //animator stuff
    public Animator resourceAnimator;
    public ParticleSystem particles;
    public GameObject deathParticle;

    private void Start()
    {
        maxHp = curHP.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HitResourceServerRpc(int resourceHP = 1)
    {
        curHP.Value -= resourceHP;
        if (particles)
        {
            particles.Play();
        }


        if (curHP.Value <= 0)
        {
            if (item.itemName != "wood")
            {
                foreach (var col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }

                DestroyResource();
            }
            else
                DestroyResourceAnimationClientRpc();

            curHP.Value = maxHp;
        }
    }

    public void DestroyResource()
    {
        for(int i = 0; i < resourcesQuantity; i++)
        {
            DropResourceServerRpc(transform.position + new Vector3(Random.Range(-2f, 2f), 2, Random.Range(-2f, 2f)));
        }
        
        if(deathParticle) Instantiate(deathParticle, transform.position, Quaternion.identity);

        Invoke(nameof(RespawnResource), respawnTimer);
        
        gameObject.SetActive(false);
    }

    private void RespawnResource()
    {
        gameObject.SetActive(true);

        if(resourceAnimator)
            RespawnResourceAnimationClientRpc();
        else
        {
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
            }
        }

    }
    
    [ClientRpc]
    public void DestroyResourceAnimationClientRpc()
    {
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        resourceAnimator.SetBool("Fall", true);
    }

    [ClientRpc]
    public void RespawnResourceAnimationClientRpc()
    {
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }

        resourceAnimator.SetBool("Fall", false);
    }


    [ServerRpc(RequireOwnership = false)]
    public void DropResourceServerRpc(Vector3 pos)
    {
        var worldGameObject = Instantiate(item.worldPrefab, pos, Quaternion.identity);

        worldGameObject.name = item.itemName;

        var worldGameObjectInvItem = worldGameObject.GetComponent<InventoryGroundItem>();
        worldGameObjectInvItem.inventoryItem = item;
        worldGameObjectInvItem.amount.Value = 1;
        worldGameObjectInvItem.Durability = 0;

        worldGameObject.GetComponent<NetworkObject>().Spawn();
        worldGameObject.GetComponent<Rigidbody>().AddForce(worldGameObject.transform.up + new Vector3(Random.Range(-0.1f, 0.1f), 0.5f, Random.Range(-0.1f, 0.1f)), ForceMode.Impulse);
    }
}