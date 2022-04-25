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

        playerTarget = GetNearPlayer(5);
        
        agent.SetDestination(playerTarget.transform.position);
    }
    
    private void AttackUpdate()
    {
        agent.SetDestination(playerTarget.transform.position);
        
        if (Time.time - lastAttack > 1 / attackRate)
        {
            if (agent.remainingDistance < 0.5f)
            {
                playerTarget.GetComponent<Player>().TakeDamageServerRpc(damage);
                lastAttack = Time.time;
            }
        }
    }
    
    private void AttackEnd() { }
    
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
