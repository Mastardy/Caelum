using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public partial class Player : NetworkBehaviour
{
    public static List<Player> allPlayers = new();
    
    private GameOptionsScriptableObject gameOptions;
    
    [Header("Player")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform weaponCamera;

    private void Start()
    {
        allPlayers.Add(this);
        
        GetItemsAndRecipes();
        
        if (!IsLocalPlayer)
        {
            firstPersonAnimator.enabled = false;
            playerCamera.gameObject.SetActive(false);
            weaponCamera.gameObject.SetActive(false);
            playerCanvas.gameObject.SetActive(false);
        }
        else
        {
            SetHealthServerRpc(maxHealth);

            currentHunger = maxHunger;
            currentThirst = maxThirst;

            foreach (var hotbar in hotbars)
            {
                hotbar.Selected = false;
            }

            gameOptions = GameManager.Instance.gameOptions;

            playerCanvas.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(true);
            playerCamera.GetComponent<Camera>().fieldOfView = 60 + (gameOptions.fieldOfView - 90f) * 0.875f;

            weaponCamera.gameObject.SetActive(true);
            weaponCamera.GetComponent<Camera>().fieldOfView = playerCamera.GetComponent<Camera>().fieldOfView;

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
            SpawnPlayer();
            GiveItemServerRpc(this, "axe_stone", 1, 1);
            GiveItemServerRpc(this, "axe_iron", 1, 1);
            GiveItemServerRpc(this, "pickaxe_stone", 1, 1);
            GiveItemServerRpc(this, "pickaxe_iron", 1, 1);
            GiveItemClientRpc(this, "bow", 1, 1);
            GiveItemServerRpc(this, "sword", 1, 1);
            GiveItemServerRpc(this, "spear_wood", 1, 1);
            GiveItemServerRpc(this, "spear_stone", 1, 1);
            GiveItemServerRpc(this, "spear_iron", 1, 1);
            GiveItemServerRpc(this, "arrow_wood", 1);
            GiveItemServerRpc(this, "arrow_stone", 1);
            GiveItemServerRpc(this, "arrow_iron", 1);
            // GiveItemClientRpc("grappling_hook", 1, 1);
            Invoke(nameof(LateLateStart), 0.1f);
        }
    }

    private void LateLateStart()
    {
        CurrentSlot = 0;
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
                else if(inCrafting) HideCrafting();
                else if(inOven) HideOven();
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

            if (takeInput)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentSlot = 0;
                if (Input.GetKeyDown(KeyCode.Alpha2)) CurrentSlot = 1;
                if (Input.GetKeyDown(KeyCode.Alpha3)) CurrentSlot = 2;
                if (Input.GetKeyDown(KeyCode.Alpha4)) CurrentSlot = 3;
                if (Input.GetKeyDown(KeyCode.Alpha5)) CurrentSlot = 4;
                if (Input.GetKeyDown(KeyCode.Alpha6)) CurrentSlot = 5;

                if (Input.GetAxis("Mouse ScrollWheel") != 0) CurrentSlot += Input.GetAxis("Mouse ScrollWheel") < 0 ? 1 : -1;
            }

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
