using UnityEngine;
using UnityEngine.UIElements;

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
    
    private void EyeTrace()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, 4, resourceMask))
        {
            if (hitInfo.transform.TryGetComponent(out Resource resource))
            {
                aimText.SetText(resource.resourceName);
                lookingAt = resource.gameObject;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);
    }
    
    private void EyeTraceInfo()
    {
        if (lookingAt == null) return;
        
        if (lookingAt.TryGetComponent(out Resource resource))
        {
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                resource.HitResourceServerRpc(this, resource.resourceName, 2);
            }
        }
    }
}
