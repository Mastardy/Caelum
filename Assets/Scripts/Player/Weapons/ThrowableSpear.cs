using Unity.Netcode;
using UnityEngine;

public class ThrowableSpear : MonoBehaviour
{
    [HideInInspector] public int damage;
    public NetworkBehaviourReference player;
    private bool cringeFlag;
    private GameObject animal;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(cringeFlag) return;
        
        if (collision.gameObject.CompareTag("Animal"))
        {
            collision.gameObject.GetComponent<Animal>().TakeDamageServerRpc(damage, player);
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<Collider>().enabled = false;
            animal = collision.gameObject;
            Invoke(nameof(StickToAnimal), 0.1f);
            cringeFlag = true;
            return;
        }

        Destroy(this);
    }

    private void StickToAnimal()
    {
        gameObject.transform.SetParent(animal.transform);
    }
}
