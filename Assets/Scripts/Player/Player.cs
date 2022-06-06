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
        
        GetItemsAndRecipes();
        
        if (!IsLocalPlayer)
        {
            firstPersonAnimator.enabled = false;
            playerCamera.gameObject.SetActive(false);
        }
        else
        {
            SetHealthServerRpc(maxHealth);

            currentHunger = maxHunger;
            currentThirst = maxThirst;

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

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        allPlayers.Clear();
    }

    private void LateStart()
    {
        if (IsLocalPlayer)
        {
            CurrentSlot = 0;
            playerCanvas.gameObject.SetActive(true);
            SpawnPlayer();
            GiveItemServerRpc(this, "axe_stone", 1, 1);
            GiveItemServerRpc(this, "axe_iron", 1, 1);
            GiveItemServerRpc(this, "pickaxe_stone", 1, 1);
            GiveItemServerRpc(this, "pickaxe_iron", 1, 1);
            // GiveItemClientRpc("bow", 1, 1);
            GiveItemServerRpc(this, "sword", 1, 1);
            GiveItemServerRpc(this, "spear_wood", 1, 1);
            GiveItemServerRpc(this, "spear_stone", 1, 1);
            GiveItemServerRpc(this, "spear_iron", 1, 1);
            // GiveItemClientRpc("grappling_hook", 1, 1);
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

            if (takeInput)
            {
                if (InputHelper.GetKeyDown(KeyCode.P, 0.1f))
                {
                    if (!isFishing) StartFishing();
                    else StopFishing();
                }

                if (!isTethered && !isTetheredPlus)
                {
                    if (InputHelper.GetKeyDown(KeyCode.R, 0.2f))
                    {
                        BeginGrapple();
                    }

                    if (InputHelper.GetKeyDown(KeyCode.Q, 0.1f))
                    {
                        BeginGrapplePlus();
                    }
                }
                else if (isTetheredPlus)
                {
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        EndGrapplePlus();
                    }
                }
            }

            if (!inInventory && !inCrafting && !inOven && !inPause)
            {
                if (InputHelper.GetKeyDown(gameOptions.chatKey, 0.1f))
                {
                    OpenChat();
                }
            }

            if (!inChat && !inCrafting && !inOven && !inPause)
            {
                if (InputHelper.GetKeyDown(gameOptions.inventoryKey, 0.1f))
                {
                    if (inInventory) HideInventory();
                    else OpenInventory();
                }
            }

            if (InputHelper.GetKeyDown(KeyCode.Escape, 0.1f))
            {
                if(inChat) HideChat();
                else if(inInventory) HideInventory();
                else if(inPause) HidePauseMenu();
                else OpenPauseMenu();
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

            if (Input.GetAxis("Mouse ScrollWheel") != 0) CurrentSlot += Input.GetAxis("Mouse ScrollWheel") < 0 ? 1 : -1;

            MovementUpdate();

            AimUpdate();

            if(takeInput) EyeTraceInfo();
            
            StatusUpdate();

            NetworkAnimatorUpdateServerRpc();
        }

        PlayFootstepSounds();
        
        AnimatorUpdate();
    }
}
