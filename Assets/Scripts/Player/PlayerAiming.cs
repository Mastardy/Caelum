using UnityEngine;
using Unity.Netcode;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private float mouseSensitivity = 30f;

    private Transform playerCamera;

    private float xRotation;

    private void Start()
    {
        if (IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Confined;
            playerCamera = GetComponentInChildren<Camera>().gameObject.transform;
        }
        else Destroy(this);
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
