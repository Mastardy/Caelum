using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public partial class Animal
{
    private Player playerTarget;
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackRate = 0.5f;
    private float lastAttack;
    [SerializeField] private float attackRange = 3.0f;
    
    private AnimalState AttackState()
    {        
        var tempAttackState = new AnimalState();
        
        tempAttackState.onStart.AddListener(AttackStart);
        tempAttackState.onUpdate.AddListener(AttackUpdate);
        tempAttackState.onEnd.AddListener(AttackEnd);

        return tempAttackState;
    }
    
    private void AttackStart()
    {
        agent.speed = fleeSpeed;
        agent.stoppingDistance = 3.0f;
        
        playerTarget = GetNearPlayer(10);
        
        agent.SetDestination(playerTarget.transform.position);
    }
    
    private void AttackUpdate()
    {
        agent.SetDestination(playerTarget.transform.position);

        agent.isStopped = false;

        if (Vector3.Distance(transform.position, playerTarget.transform.position) > attackRange) return;
        
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        
        if (Time.time - lastAttack > 1 / attackRate)
        {
            animator.SetTrigger(attackCache);
            lastAttack = Time.time;
        }
    }

    private void AttackEnd()
    {
        agent.isStopped = false;
        agent.stoppingDistance = 0.1f;
    }
    
    private Player GetNearPlayer(float distance)
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (Vector3.Distance(player.transform.position, transform.position) < distance)
            {
                return player;
            }
        }

        return null;
    }

    public void TryAttack()
    {
        var results = new Collider[10];

        if (Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + transform.forward * attackRange, 2, results) > 1)
        {
            foreach(var col in results)
            {
                if (!col) continue;
                if (col.CompareTag("Player"))
                {
                    playerTarget.GetComponent<Player>().TakeDamageServerRpc(damage);
                    audioSource.PlayOneShot(attackSound);
                }
            }
        }
    }
}
