using UnityEngine;

public partial class Animal
{
    private AnimalState IdleState()
    {
        var tempIdleState = new AnimalState();
        
        tempIdleState.onStart.AddListener(IdleStart);
        tempIdleState.onUpdate.AddListener(IdleUpdate);
        tempIdleState.onEnd.AddListener(IdleEnd);

        return tempIdleState;
    }

    private void IdleStart()
    {
        agent.ResetPath();
        animator.SetBool(runningCache, false);
        animator.SetBool(walkingCache, false);
    }
    
    private void IdleUpdate() { }
    
    private void IdleEnd() { }
}
