using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        animalStates.Add(idleState, IdleState());
        animalStates.Add(roamState, RoamState());
        animalStates.Add(fleeState, FleeState());
    }

    private void Update()
    {
        ChangeState();
        animalStates[CurrentState].onUpdate.Invoke();
    }
}
