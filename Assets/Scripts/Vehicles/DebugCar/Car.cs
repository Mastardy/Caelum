using Unity.Netcode;
using UnityEngine;

public class Car : NetworkBehaviour
{
    private Player driver;

    public Transform cameraPosition;
    
    [SerializeField] private float carSpeed;
    [SerializeField] private float turnSpeed;

    [ServerRpc(RequireOwnership = false)]
    public void CarEnterServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        if (driver != null) return;

        if (player.TryGet(out Player ply))
        {
            driver = ply;
            ply.EnterCarClientRpc(this);   
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CarExitServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;

        if (player.TryGet(out Player ply))
        {
            if (ply != driver) return;

            driver = null;
            
            ply.ExitCarClientRpc(this);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void CarMovementServerRpc(NetworkBehaviourReference player, Vector2 input, float deltaTime)
    {
        if (!IsServer) return;
        
        if (player.TryGet(out Player ply))
        {
            if (driver != ply) return;

            var carTransform = transform;
            
            carTransform.position += carTransform.forward * (input.y * carSpeed * deltaTime);
            carTransform.Rotate(Vector3.up, input.x * input.y * turnSpeed * deltaTime);
        }
    }
}