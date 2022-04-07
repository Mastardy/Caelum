using UnityEngine;

public partial class Animal
{
    private AnimalState IdleState()
    {
        var tempIdleState = new AnimalState();
        
        tempIdleState.OnStart.AddListener(IdleStart);
        tempIdleState.OnUpdate.AddListener(IdleUpdate);
        tempIdleState.OnEnd.AddListener(IdleEnd);

        return tempIdleState;
    }
    
    private void IdleStart() { }
    
    private void IdleUpdate() { }
    
    private void IdleEnd() { }
}
