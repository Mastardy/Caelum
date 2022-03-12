using UnityEngine;
using Unity.Netcode;

public partial class Player : NetworkBehaviour
{
    [Header("Player")]
    private Transform playerCamera;
    
    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>().transform;
        
        if (!IsLocalPlayer)
        {
            playerCanvas.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(false);
            enabled = false;
            GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            var cameraMain = Camera.main;
            if(cameraMain != null)
                if(cameraMain != playerCamera.GetComponent<Camera>())
                    cameraMain.gameObject.SetActive(false);   
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        EyeTrace();
    }
    
    private void Update()
    {
        if (!IsLocalPlayer) return;

        MovementUpdate();

        AimUpdate();
        
        EyeTraceInfo();
    }
}
