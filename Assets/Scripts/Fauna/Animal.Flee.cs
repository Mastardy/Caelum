using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public partial class Animal
{
    [SerializeField] private float fleeSpeed = 9f;
    
    private AnimalState FleeState()
    {
        var tempFleeState = new AnimalState();
        
        tempFleeState.onStart.AddListener(FleeStart);
        tempFleeState.onUpdate.AddListener(FleeUpdate);
        tempFleeState.onEnd.AddListener(FleeEnd);

        return tempFleeState;
    }
    
    private void FleeStart()
    {
        animator.SetBool(runningCache, true);
        
        agent.speed = fleeSpeed;
        
        var animalPosition = transform.position;

        NavMesh.SamplePosition(animalPosition + GetFleeDirection() * 12, out NavMeshHit navMeshHit, 15,  1);
        
        agent.SetDestination(navMeshHit.position);
    }
    
    private void FleeUpdate()
    {
        if (!PlayerIsNear(5)) return;

        FleeStart();
    }

    private void FleeEnd()
    {
        animator.SetBool(runningCache, false);
    }

    private Vector3 GetFleeDirection()
    {
        var nearPlayersAngles = new List<float>();

        foreach (var ply in Player.allPlayers)
        {
            Vector3 direction = transform.position - ply.transform.position;
            if (direction.magnitude < 5)
            {
                nearPlayersAngles.Add(Mathf.Atan2(direction.z, direction.x));
            }
        }

        var averageAngle = nearPlayersAngles.Sum() / nearPlayersAngles.Count;

        return new Vector3(Mathf.Cos(averageAngle), 0, Mathf.Sin(averageAngle));
    }
}
