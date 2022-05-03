using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        maxHealth += Random.Range(-20, 21);
        currentHealth.Value = maxHealth;
        
        animalStates.Add(idleState, IdleState());
        animalStates.Add(roamState, RoamState());
        animalStates.Add(fleeState, FleeState());
        animalStates.Add(attackState, AttackState());
    }

    private void Update()
    {
        if (!dead) ChangeState();
        animalStates[CurrentState].onUpdate.Invoke();
    }
}
