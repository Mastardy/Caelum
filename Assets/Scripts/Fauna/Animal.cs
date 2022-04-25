using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public partial class Animal : NetworkBehaviour
{
    private NavMeshAgent agent;

    public float maxHealth = 150f;
    public NetworkVariable<float> currentHealth = new(readPerm: NetworkVariableReadPermission.Everyone);

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        animalStates.Add(idleState, IdleState());
        animalStates.Add(roamState, RoamState());
        animalStates.Add(fleeState, FleeState());
        animalStates.Add(attackState, AttackState());
    }

    private void Update()
    {
        ChangeState();
        animalStates[CurrentState].onUpdate.Invoke();
    }
}
