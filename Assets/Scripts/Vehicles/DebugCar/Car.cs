using Unity.Netcode;
using UnityEngine;

public class Car : NetworkBehaviour
{
    private Player driver;

    public Transform cameraPosition;
    
    [SerializeField] private float carSpeed;
    [SerializeField] private float turnSpeed;

    [ServerRpc]
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
        
        Debug.Log(input);

        if (player.TryGet(out Player ply))
        {
            if (driver != ply) return;

            gameObject.transform.position += gameObject.transform.forward * input.y * carSpeed * Time.deltaTime;
            gameObject.transform.Rotate(Vector3.up, input.x * turnSpeed * Time.deltaTime);
        }
    }
}