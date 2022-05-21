using UnityEngine;
using UnityEngine.AI;

public partial class Animal
{
    [SerializeField] private float roamSpeed = 5f;

    private AnimalState RoamState()
    {
        var tempRoamState = new AnimalState();
        
        tempRoamState.onStart.AddListener(RoamStart);
        tempRoamState.onUpdate.AddListener(RoamUpdate);
        tempRoamState.onEnd.AddListener(RoamEnd);

        return tempRoamState;
    }
    
    private void RoamStart()
    {        
        agent.speed = roamSpeed;
        
        var randomPosition = Random.insideUnitSphere * Random.Range(10.0f, 20.0f);

        NavMesh.SamplePosition(transform.position + randomPosition, out NavMeshHit navMeshHit, 25f,  1);
        
        agent.SetDestination(navMeshHit.position);
    }

    private void RoamUpdate()
    {
        if (agent.remainingDistance < 1) RoamStart();
    }

    private void RoamEnd()
    {
        
    }
}
