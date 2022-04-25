using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public partial class Animal
{
    private Player playerTarget;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackRate = 0.5f;
    private float lastAttack;
    
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
        agent.stoppingDistance = 1.0f;
        
        playerTarget = GetNearPlayer(5);
        
        agent.SetDestination(playerTarget.transform.position);
    }
    
    private void AttackUpdate()
    {
        agent.SetDestination(playerTarget.transform.position);

        if (agent.remainingDistance > 1.0f) return;
        
        if (Time.time - lastAttack > 1 / attackRate)
        {
            playerTarget.GetComponent<Player>().TakeDamageServerRpc(damage);
            lastAttack = Time.time;
        }
    }

    private void AttackEnd()
    {
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
}
