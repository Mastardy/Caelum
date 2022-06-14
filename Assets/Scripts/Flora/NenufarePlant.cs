using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NenufarePlant : MonoBehaviour
{
    private Animator animator;
    private Player player;
    public int cameraShakeAmount = 10;
    public int damage = 25;
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
            player.cameraShake.StopShake();
            player = null;
            animator.SetBool("Close", false);
        }
    }

    public void StartShake()
    {
        player.cameraShake.StartShake(1f, cameraShakeAmount);
    }

    public void Attack()
    {
        if (player)
        {
            player.TakeDamageServerRpc(damage);
            player.cameraShake.StopShake();
        }
    }
}
