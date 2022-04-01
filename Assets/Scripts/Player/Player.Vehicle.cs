using UnityEngine;

public partial class Player
{
    private GameObject car;
    
    private void CarMovement()
    {
        Vector2 carInput = Vector2.zero;
        carInput.x = Input.GetKey(gameOptions.rightKey) ? 1 : Input.GetKey(gameOptions.leftKey) ? -1 : 0;
        carInput.y = Input.GetKey(gameOptions.forwardKey) ? 1 : Input.GetKey(gameOptions.backwardKey) ? -1 : 0;

        car.GetComponent<Car>().CarMovementServerRpc(gameObject, carInput);
    }
}
