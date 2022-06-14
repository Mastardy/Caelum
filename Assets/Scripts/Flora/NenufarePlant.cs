using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NenufarePlant : MonoBehaviour
{
    private Animator animator;
    private Player player;
    public int cameraShakeAmount = 10;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.GetComponent<Player>();
            player.cameraShake.StartShake(1f, cameraShakeAmount);
            animator.SetBool("Close",true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.cameraShake.StopShake();
            player = null;
            animator.SetBool("Close", false);
        }
    }

    public void Attack()
    {
        if (player)
        {
            player.TakeDamageServerRpc(50);
            player.cameraShake.StopShake();
        }
    }
}
