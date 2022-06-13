using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NenufarePlant : MonoBehaviour
{
    private Animator animator;
    private Player player;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
            animator.SetBool("Close",true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
            animator.SetBool("Close", false);
        }
    }

    public void Attack()
    {
        if (player)
        {
            player.TakeDamageServerRpc(50);
        }
    }
}
