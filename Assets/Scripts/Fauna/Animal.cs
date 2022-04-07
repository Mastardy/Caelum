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
        if (Player.localPlayer != null)
        {
            stateText.transform.LookAt(Player.localPlayer.transform);
            stateText.transform.Rotate(Vector3.up, 180);
        }
        ChangeState();
        animalStates[CurrentState].OnUpdate.Invoke();
    }
}
