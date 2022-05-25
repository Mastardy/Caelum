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
                for (int i = 0; i < hotbars.Count; i++)
                {
                    hotbars[i].Selected = false;
                }
                
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
        GiveItemClientRpc("axe_stone", 1, 1);
        GiveItemClientRpc("axe_iron", 1, 1);
        GiveItemClientRpc("pickaxe_stone", 1, 1);
        GiveItemClientRpc("pickaxe_iron", 1, 1);
        GiveItemClientRpc("bow", 1, 1);
        GiveItemClientRpc("sword", 1, 1);
        GiveItemClientRpc("spear_wood", 1, 1);
        GiveItemClientRpc("spear_stone", 1, 1);
        GiveItemClientRpc("spear_iron", 1, 1);
        GiveItemClientRpc("grappling_hook", 1, 1);
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

            if (InputHelper.GetKeyDown(KeyCode.Q, 0.1f))
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
            if (Input.GetKeyDown(KeyCode.Alpha6)) CurrentSlot = 5;

            MovementUpdate();

            AimUpdate();

            EyeTraceInfo();
            
            StatusUpdate();

            //ANDR�, REVER ESSE CODIGO QUE ATUALIZA A BOOL QUE INDICA QUE O WORLD MODEL TA SEGURNADO UMA TOOL
            bool holdTool = false;
            if (!handIsEmpty)
            {
                holdTool = hotbars[currentSlot].slot.inventoryItem.itemTag is ItemTag.Axe or ItemTag.Pickaxe or ItemTag.Sword;
                thirdPersonAnimator.SetLayerWeight(2, holdTool? 0: 1);

                if (hotbars[currentSlot].slot.inventoryItem.itemTag is ItemTag.Bow or ItemTag.Spear)
                {
                    AnimatorAim(InputHelper.GetKey(gameOptions.secondaryAttackKey));
                }
            }

            NetworkAnimatorUpdateServerRpc(isCrouched, horizontalVelocity.magnitude, input.x, input.y, 
                isGrounded, xRotation, verticalVelocity, holdTool, false, inParachute);
        }

        PlayFootstepSounds();
        
        AnimatorUpdate();
    }
}
