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
        SetHealthServerRpc(maxHealth);
        
        allPlayers.Add(this);
        
        GetInventoryItems();
        
        if (!IsLocalPlayer)
        {
            firstPersonAnimator.enabled = false;
        }
        else
        {
            if (IsLocalPlayer)
            {
                gameOptions = GameManager.Instance.gameOptions;

                playerCanvas.gameObject.SetActive(true);
                playerCamera.gameObject.SetActive(true);

                characterController = GetComponent<CharacterController>();
                Cursor.lockState = CursorLockMode.Locked;

                AnimatorStart();
            }
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
            HUDUpdate();
            
            chatBox.SetActive(!inInventory && !inCrafting && !inOven);

            if (InputHelper.GetKeyDown(KeyCode.P, 0.1f))
            {
                if (!isFishing) StartFishing();
                else StopFishing();
            }
            
            if (InputHelper.GetKeyDown(KeyCode.J, 0.5f))
            {
                TakeDamageServerRpc(5);
            }
            
            if (InputHelper.GetKeyDown(KeyCode.K, 0.5f))
            {
                TakeDamageServerRpc(-5);
            }
            
            if (!inInventory && !inCrafting && !inOven)
            {
                if (InputHelper.GetKeyDown(gameOptions.chatKey, 0.1f))
                {
                    OpenChat();
                }
            }
            
            if (!inChat && !inCrafting && !inOven)
            {
                if (InputHelper.GetKeyDown(gameOptions.inventoryKey, 0.1f))
                {
                    if (inInventory) HideInventory();
                    else OpenInventory();
                }
            }

            if (inCrafting)
            {
                if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
                {
                    craftingTable.CloseCraftingServerRpc(this);
                }
            }

            if (inOven)
            {
                OvenUpdate();
                
                if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
                {
                    cooker.CloseOvenServerRpc(this);
                }
            }
            
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
