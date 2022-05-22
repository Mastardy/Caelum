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
    private const string attackState = "Attack";
    
    private void ChangeState()
    {
        switch (CurrentState)
        {
            case idleState:
                CurrentState = PlayerIsNear(5) 
                    ? fleeState// Random.Range(0, 2) == 0 ? fleeState : attackState 
                    : RandomState();
                break;
            case roamState:
                CurrentState = PlayerIsNear(5) 
                    ? attackState // Random.Range(0, 2) == 0 ? fleeState : attackState 
                    : RandomState();
                break;
            case attackState:
                if (!playerTarget) CurrentState = RandomState();
                else
                {
                    if (playerTarget.currentHealth.Value <= 0) CurrentState = RandomState();
                    else if (Vector3.Distance(transform.position, playerTarget.transform.position) > 10) CurrentState = RandomState();
                }
                break;
            case fleeState:
                if (!PlayerIsNear(15)) CurrentState = RandomState();
                break;
            default:
                CurrentState = RandomState();
                break;
        }
    }
    
    private bool PlayerIsNear(float distance)
    {
        foreach (var player in Player.allPlayers)
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

        var random = new System.Random();
        
        switch (random.Next(0, 2))
        {
            case 0:
                transitionDelay = (float)(random.NextDouble() * random.Next(2, 5));
                currentRandomState = idleState;
                return idleState;
            case 1:
                transitionDelay = (float)(random.NextDouble() * random.Next(5, 10));
                currentRandomState = roamState;
                return roamState;
        }

        return idleState;
    }
}
