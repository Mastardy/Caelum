using System;
using Unity.Netcode;
using UnityEngine;

public class ThrowableSpear : MonoBehaviour
{
    [HideInInspector] public int damage;
    public NetworkBehaviourReference player;
    private bool cringeFlag;
    private Animal animal;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(cringeFlag) return;
        
        if (collision.gameObject.CompareTag("Animal"))
        {
            collision.gameObject.GetComponent<Animal>().TakeDamageServerRpc(damage, player);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            animal = collision.gameObject.GetComponent<Animal>();
            if(animal) transform.SetParent(animal.transform);
            transform.localPosition = transform.localPosition + transform.forward * 0.1f;
            animal.onDestroy.AddListener(() =>
            {
                transform.parent = null;
                Destroy(this);
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Collider>().enabled = true;
            });
            cringeFlag = true;
            return;
        }

        Destroy(this);
    }
}
