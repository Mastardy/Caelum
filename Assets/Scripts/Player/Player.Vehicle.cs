using UnityEngine;
using Unity.Netcode;

public partial class Player
{
    private Car car;

    private float lastUse;
    
    private void CarMovement()
    {
        if (car == null) return;
        
        Vector2 carInput = Vector2.zero;
        carInput.x = Input.GetKey(gameOptions.rightKey) && Input.GetKey(gameOptions.leftKey) ? 0 :
            Input.GetKey(gameOptions.rightKey) ? 1 :
            Input.GetKey(gameOptions.leftKey) ? -1 : 0;
        carInput.y = Input.GetKey(gameOptions.backwardKey) && Input.GetKey(gameOptions.forwardKey) ? 0 :
            Input.GetKey(gameOptions.forwardKey) ? 1 :
            Input.GetKey(gameOptions.backwardKey) ? -1 : 0;

        playerCamera.transform.position = car.cameraPosition.position;
        playerCamera.transform.rotation = car.cameraPosition.rotation;
        
        car.CarMovementServerRpc(this, carInput, Time.deltaTime);

        if (Input.GetKeyDown(gameOptions.useKey) && Time.time - lastUse > 0.2f)
        {
            car.CarExitServerRpc(this);
        }
    }

    [ClientRpc]
    public void EnterCarClientRpc(NetworkBehaviourReference netCar)
    {
        if (netCar.TryGet(out Car vehicle))
        {
            car = vehicle;
        }
    }

    [ClientRpc]
    public void ExitCarClientRpc(NetworkBehaviourReference netCar)
    {
        if (netCar.TryGet(out Car _))
        {
            car = null;
            playerCamera.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
}
