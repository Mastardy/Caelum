using Unity.Netcode;
using UnityEngine;

public class SpoikyPlant : MonoBehaviour
{
    private Animator animator;
    private Player player;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //checar se ainda ta dentro do collider e fechar dnovo

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
            Invoke("Close", Random.Range(1f, 3f));
            Debug.Log("entrou");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Open();
            player = null;
        }
    }

    private void Close()
    {
        animator.SetBool("Close", true);
    }
    
    private void Open()
    {
        animator.SetBool("Close", false);
    }

    public void Damage()
    {
        if (player)
            player.TakeDamageServerRpc(10);
        Invoke("Open", Random.Range(1f, 3f));
    }
}
