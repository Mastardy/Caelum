using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public partial class Player : NetworkBehaviour
{
    public static List<Player> allPlayers = new();
    
    private GameOptionsScriptableObjects gameOptions;
    
    [Header("Player")]
    [SerializeField] private Transform playerCamera;
    
    private void Start()
    {
        allPlayers.Add(this);

        Debug.Log(allPlayers.Count);
        
        if (!IsLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
            firstPersonAnimator.enabled = false;
        }
        else
        {
            gameOptions = GameManager.Instance.gameOptions;
            
            playerCanvas.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(true);
            
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            
            AnimatorStart();
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;

        EyeTrace();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            if (InputHelper.GetKeyDown(gameOptions.chatKey, 0.1f)) OpenChat();

            if (car != null)
            {
                CarMovement();

                return;
            }

            MovementUpdate();

            AimUpdate();

            EyeTraceInfo();
            
            NetworkAnimatorUpdateServerRpc(isCrouched, horizontalVelocity.magnitude, input.x, input.y, 
                isGrounded, xRotation, verticalVelocity);
        }

        AnimatorUpdate();
    }
}
