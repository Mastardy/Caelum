using Unity.Netcode;
using UnityEngine;

public class Car : NetworkBehaviour
{
    [HideInInspector] public GameObject driver;

    private float carSpeed;

    [ServerRpc]
    public void CarMovementServerRpc(GameObject player, Vector2 input)
    {
        if (!IsServer) return;

        if (player != driver) return;

        gameObject.transform.position += Vector3.right * input.x * carSpeed * Time.deltaTime;
        gameObject.transform.position += Vector3.forward * input.y * carSpeed * Time.deltaTime;
    }
}