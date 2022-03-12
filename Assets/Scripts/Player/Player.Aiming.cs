using UnityEngine;

public partial class Player
{
    [Header("Aiming")]
    [SerializeField] private float mouseSensitivity = 30f;
    [SerializeField] private Transform headTransform;

    private float xRotation;

    private void AimUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        var headRotation = Mathf.Clamp(xRotation * 2, -45, 35f);
        
        headTransform.localRotation = Quaternion.Euler(headRotation, 0f, 0f);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
