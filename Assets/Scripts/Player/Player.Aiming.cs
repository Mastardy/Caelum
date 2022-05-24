using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Aiming")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Vector3 parachuteCameraRotation = new Vector3(-25, 0, 0);

    private float xRotation;

    private void AimUpdate()
    {
        float mouseX = 0;
        float mouseY = 0;

        if (takeInput)
        {
            mouseX = Input.GetAxis("Mouse X") * gameOptions.mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * gameOptions.mouseSensitivity;
        }

        if (inParachute)
        {
            headTransform.localRotation = Quaternion.Euler(parachuteCameraRotation);

            playerCamera.localRotation = Quaternion.Euler(parachuteCameraRotation.x *-2, 0f, 0f);
        }
        else
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -85f, 85f);

            var headRotation = Mathf.Clamp(xRotation * 2, -45, 35f);

            headTransform.localRotation = Quaternion.Euler(headRotation, 0f, 0f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);
    }

    private void EyeTrace()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, 3, hitMask))
        {
            if (hitInfo.transform.TryGetComponent(out Car vehicle))
            {
                aimText.SetText(vehicle.name);
                lookingAt = vehicle.gameObject;
                return;
            }
            
            if (hitInfo.transform.TryGetComponent(out Resource resource))
            {
                aimText.SetText(resource.name);
                lookingAt = resource.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out InventoryGroundItem groundItem))
            {
                aimText.SetText(groundItem.name + "\n" + groundItem.amount + "x");
                lookingAt = groundItem.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out CraftingTable craftTable))
            {
                aimText.SetText("Crafting Table");
                lookingAt = craftTable.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Oven oven))
            {
                aimText.SetText("Oven");
                lookingAt = oven.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Animal animal))
            {
                aimText.SetText(animal.currentHealth.Value.ToString("F0") + "/" + animal.maxHealth.ToString("F0"));
                lookingAt = animal.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out FishingNet fishingNet))
            {
                aimText.SetText("Fishing Net" + (fishingNet.fishesInNet > 0 ? $"\n{fishingNet.fishesInNet}" : String.Empty));
                lookingAt = fishingNet.gameObject;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);
    }
    
    private void EyeTraceInfo()
    {
        if (hotbars[currentSlot].slot.inventoryItem)
        {
            switch (hotbars[currentSlot].slot.inventoryItem.itemTag)
            {
                case ItemTag.Sword:
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.6f))
                    {
                        AnimatorUseSword();
                        
                        if (!lookingAt) return;
                        
                        if (lookingAt.TryGetComponent(out Animal animal))
                        {
                            animal.TakeDamageServerRpc(20, this);
                        }
                    }
                    break;
                case ItemTag.Spear:
                    if (InputHelper.GetKey(gameOptions.secondaryAttackKey))
                    {
                        if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.3f))
                        {
                            AnimatorThrowSpear();
                        }
                        break;
                    }
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.3f))
                    {
                        AnimatorUseSpear();
                        
                        if (!lookingAt) return;
                        
                        if (lookingAt.TryGetComponent(out Animal animal))
                        {
                            animal.TakeDamageServerRpc(10, this);
                        }
                    }
                    break;
                case ItemTag.Bow:
                    if (!Input.GetKey(gameOptions.secondaryAttackKey)) break;
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.5f))
                    {
                        AnimatorShootBow();
                        
                        if (!lookingAt) return;
                        
                        if (lookingAt.TryGetComponent(out Animal animal))
                        {
                            animal.TakeDamageServerRpc(50, this);
                        }
                    }
                    break;
            }
        }
        
        if (!lookingAt) return;

        if (lookingAt.TryGetComponent(out Car vehicle))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.2f))
            {
                vehicle.CarEnterServerRpc(this);
            }

            return;
        }
        
        if (lookingAt.TryGetComponent(out Resource resource))
        {
            if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.15f))
            {
                if (!hotbars[currentSlot].slot.inventoryItem) return;

                switch (resource.resourceName)
                {
                    case "wood":
                        if(hotbars[currentSlot].slot.inventoryItem.itemTag == ItemTag.Axe)
                        {
                            if (hotbars[currentSlot].slot.Durability <= 0) return;
                            hotbars[currentSlot].slot.Durability -= 0.1f;
                            AnimatorUseAxe();
                            resource.HitResourceServerRpc(this, 5);
                        }
                        break;
                    case "stone":
                        if (hotbars[currentSlot].slot.inventoryItem.itemTag == ItemTag.Pickaxe)
                        {
                            if (hotbars[currentSlot].slot.Durability <= 0) return;
                            hotbars[currentSlot].slot.Durability -= 0.1f;
                            AnimatorUsePickaxe();
                            resource.HitResourceServerRpc(this, 3);
                        }
                        break;
                    default:
                        resource.HitResourceServerRpc(this);
                        break;
                }
            }
            else if (InputHelper.GetKeyDown(gameOptions.useKey, 1f))
            {
                if (resource.name != "Fruit") return;
                
                resource.HitResourceServerRpc(this, 1);
                AnimatorCollect();
            }
            
            return;
        }

        if (lookingAt.TryGetComponent(out InventoryGroundItem groundItem))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
            {
                groundItem.PickUpServerRpc(this);
            }
            
            return;
        }

        if (lookingAt.TryGetComponent(out CraftingTable craftTable))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
            {
                craftTable.OpenCraftingServerRpc(this);
            }

            return;
        }

        if (lookingAt.TryGetComponent(out Oven oven))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
            {
                oven.OpenOvenServerRpc(this);
            }

            return;
        }

        if (lookingAt.TryGetComponent(out FishingNet fishingNet))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.3f))
            {
                fishingNet.TryFishingServerRpc(this);
            }
        }
    }

    [ServerRpc]
    public void ThrowSpearServerRpc(NetworkBehaviourReference ply, string itemName, int slot)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            player.hotbars[currentSlot].slot.Clear();
            
            var playerCameraTransform = playerCamera.transform;
            var spear = Instantiate(weaponItems[itemName].throwablePrefab, playerCameraTransform.position + playerCameraTransform.forward,
                Quaternion.Euler(playerCameraTransform.rotation.eulerAngles + new Vector3(90, 0, 0)));

            spear.GetComponent<NetworkObject>().Spawn();
            spear.GetComponent<Rigidbody>().AddForce(playerCameraTransform.forward * 20, ForceMode.Impulse);

            spear.GetComponent<ThrowableSpear>().damage = itemName == "spear_iron" ? 30 : itemName == "spear_stone" ? 20 : 10;
            spear.GetComponent<ThrowableSpear>().player = ply;
        }
    }
}
