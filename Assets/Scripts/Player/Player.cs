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
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        
        allPlayers.Add(this);
        
        GetItemsAndRecipes();
        
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
                
                Invoke(nameof(LateStart), 0.1f);
            }
        }
    }

    private void LateStart()
    {
        SpawnPlayer();
        GiveItemClientRpc("axe_wood", 1, 1);
        GiveItemClientRpc("pickaxe_wood", 1, 1);
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

            if (Time.time - respawnTime < 0.1f)
            {
                return;
            }
            
            if(Time.time - lastSafePosition > safePositionTimer) CalculateSafePosition();

            if (currentHealth.Value < 1)
            {
                RespawnPlayer();
                return;
            }
            
            if (InputHelper.GetKeyDown(KeyCode.P, 0.1f))
            {
                if (!isFishing) StartFishing();
                else StopFishing();
            }

            if (InputHelper.GetKeyDown(KeyCode.J, 0.2f))
            {
                BeginGrapple();
            }

            if (InputHelper.GetKeyDown(KeyCode.K, 0.1f))
            {
                BeginGrapplePlus();
            }
            else if (Input.GetKeyUp(KeyCode.K))
            {
                EndGrapplePlus();
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
                if(inOvenMinigame) OvenMinigameUpdate();
                
                if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f)) 
                {
                    cooker.CloseOvenServerRpc(this);
                }
            }
            
            if (car)
            {
                CarMovement();

                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentSlot = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) CurrentSlot = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) CurrentSlot = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) CurrentSlot = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5)) CurrentSlot = 4;

            MovementUpdate();

            AimUpdate();

            EyeTraceInfo();
            
            StatusUpdate();
            
            NetworkAnimatorUpdateServerRpc(isCrouched, horizontalVelocity.magnitude, input.x, input.y, 
                isGrounded, xRotation, verticalVelocity);
        }

        AnimatorUpdate();
    }
}
