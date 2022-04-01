using Unity.Netcode;
using UnityEngine;

public class Car : NetworkBehaviour
{
    private Player driver;

    private float carSpeed;

    [ServerRpc]
    public void CarEnterServerRpc(NetworkBehaviourReference player)
    {
        if (!IsServer) return;
        if (!driver) return;

        Debug.Log("Try to make player the driver");

        if (player.TryGet(out Player ply))
        {
            driver = ply;
            ply.EnterCarClientRpc(this);   
        }
    }

    [ServerRpc]
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
    
    [ServerRpc]
    public void CarMovementServerRpc(NetworkBehaviourReference player, Vector2 input)
    {
        if (!IsServer) return;

        if (player.TryGet(out Player ply))
        {
            if (driver != ply) return;

            gameObject.transform.position += Vector3.right * input.x * carSpeed * Time.deltaTime +
                                             Vector3.forward * input.y * carSpeed * Time.deltaTime;   
        }
    }
}