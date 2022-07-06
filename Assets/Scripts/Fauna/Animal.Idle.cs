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

    private float idleSoundTimer;
    private float idleSoundDuration;
    
    private void IdleStart()
    {
        agent.ResetPath();
        audioSource.PlayOneShot(idleSound);
        idleSoundTimer = Time.time;
        idleSoundDuration = Random.Range(3.0f, 5.0f);
    }
    
    private void IdleUpdate() 
    {
        if (Time.time - idleSoundTimer > idleSoundDuration)
        {
            audioSource.PlayOneShot(idleSound);
            idleSoundTimer = Time.time;
            idleSoundDuration = Random.Range(1.0f, 3.0f);
        }
    }
    
    private void IdleEnd() { }
}
