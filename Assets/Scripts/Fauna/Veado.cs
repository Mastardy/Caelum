using System;
using Unity.Netcode;

public enum VeadoStates
{
    Idle,
    Roaming,
    Eating,
    Drinking,
    Fleeing
}

public class Veado : NetworkBehaviour
{
    private VeadoStates veadoState;
    
    private void Start()
    {
        veadoState = VeadoStates.Idle;
    }
    

    private void Update()
    {
        switch (veadoState)
        {
            case VeadoStates.Idle:
                VeadoIdle();
                break;
            case VeadoStates.Roaming:
                VeadoRoaming();
                break;
            case VeadoStates.Eating:
                VeadoEating();
                break;
            case VeadoStates.Drinking:
                VeadoDrinking();
                break;
            case VeadoStates.Fleeing:
                VeadoFleeing();
                break;
        }
    }

    private void VeadoIdle()
    {
        
    }

    private void VeadoRoaming()
    {
        
    }

    private void VeadoEating()
    {
        
    }
    
    private void VeadoDrinking()
    {
        
    }

    private void VeadoFleeing()
    {
        
    }
}
