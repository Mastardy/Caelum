using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

public class AnimalState
{
    public AnimalState()
    {
        onStart = new UnityEvent();
        onUpdate = new UnityEvent();
        onEnd = new UnityEvent();
    }
    
    public readonly UnityEvent onStart;
    public readonly UnityEvent onUpdate;
    public readonly UnityEvent onEnd;
}

public partial class Animal
{
    // TODO: REFACTOR
    private Dictionary<string, AnimalState> animalStates = new();

    private string currentState;
    private string CurrentState
    {
        get => currentState;
        set
        {
            if (currentState != null && value == currentState) return;

            if (currentState != null) animalStates[currentState].onEnd.Invoke();

            currentState = value;
            stateText.text = currentState;

            animalStates[currentState].onStart.Invoke();
        }
    }
    // TODO: REFACTOR

    [SerializeField] private TextMeshProUGUI stateText;
    
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
                return true;
            }
        }

        return false;
    }

    private string currentRandomState;
    
    private string RandomState()
    {
        if (Time.time - transitionTimer < transitionDelay) return currentRandomState;

        transitionTimer = Time.time;
        
        switch (Random.Range(0, 2))
        {
            case 0:
                transitionDelay = Random.Range(2f, 5f);
                currentRandomState = idleState;
                return idleState;
            case 1:
                transitionDelay = Random.Range(5f, 10f);
                currentRandomState = roamState;
                return roamState;
            default:
                transitionDelay = Random.Range(2f, 5f);
                currentRandomState = idleState;
                return idleState;
        }
    }
}
