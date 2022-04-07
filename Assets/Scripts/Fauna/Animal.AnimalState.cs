using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class AnimalState
{
    public AnimalState()
    {
        OnStart = new();
        OnUpdate = new();
        OnEnd = new();
    }
    
    public readonly UnityEvent OnStart;
    public readonly UnityEvent OnUpdate;
    public readonly UnityEvent OnEnd;
}

public partial class Animal
{
    // TODO: REFACTOR
    private Dictionary<string, AnimalState> animalStates = new();

    private string currentState;
    private string CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState != null && value == currentState) return;

            if (currentState != null) animalStates[currentState].OnEnd.Invoke();

            currentState = value;

            animalStates[currentState].OnStart.Invoke();
        }
    }
    // TODO: REFACTOR
    
    private float transitionTimer;
    private float transitionDelay;
    
    private const string idleState = "Idle";
    private const string roamState = "Roam";
    private const string fleeState = "Flee";
    
    private void ChangeState()
    {
        switch (CurrentState)
        {
            case idleState:
                CurrentState = PlayerIsNear(5) ? fleeState : RandomState();
                break;
            case roamState:
                CurrentState = PlayerIsNear(5) ? fleeState : RandomState();
                break;
            case fleeState:
                if (agent.remainingDistance < 0.5f || !PlayerIsNear(15)) CurrentState = RandomState();
                break;
            default:
                CurrentState = RandomState();
                break;
        }
    }

    private bool PlayerIsNear(float distance)
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (Vector3.Distance(player.transform.position, transform.position) < distance)
            {
                predator = player;
                return true;
            }
        }

        return false;
    }

    private string RandomState()
    {
        if (Time.time - transitionTimer < transitionDelay) return CurrentState;

        transitionTimer = Time.time;
        
        switch (Random.Range(0, 2))
        {
            case 0:
                transitionDelay = Random.Range(2f, 5f);
                return idleState;
            case 1:
                transitionDelay = Random.Range(5f, 10f);
                return roamState;
            default:
                transitionDelay = Random.Range(2f, 5f);
                return idleState;
        }
    }
}
