using UnityEngine;
using UnityEngine.AI;

public partial class Animal
{
    private Player predator;

    [SerializeField] private float fleeSpeed = 9f;
    
    private AnimalState FleeState()
    {
        var tempFleeState = new AnimalState();
        
        tempFleeState.OnStart.AddListener(FleeStart);
        tempFleeState.OnUpdate.AddListener(FleeUpdate);
        tempFleeState.OnEnd.AddListener(FleeEnd);

        return tempFleeState;
    }
    
    private void FleeStart()
    {
        agent.speed = fleeSpeed;
        
        var animalPosition = transform.position;

        NavMesh.SamplePosition(animalPosition + (animalPosition - predator.transform.position).normalized * 12, out NavMeshHit navMeshHit, 5,  1);
        
        agent.SetDestination(navMeshHit.position);
    }
    
    private void FleeUpdate()
    {
        if (!PlayerIsNear(5)) return;

        FleeStart();
    }
    
    private void FleeEnd() { }
}
