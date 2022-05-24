using Unity.Netcode;
using UnityEngine;

public class ThrowableSpear : MonoBehaviour
{
    [HideInInspector] public int damage;
    public NetworkBehaviourReference player;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Animal"))
        {
            collision.gameObject.GetComponent<Animal>().TakeDamageServerRpc(damage, player);
        }

        Debug.Log(collision.gameObject);
        Destroy(this);
    }
}
