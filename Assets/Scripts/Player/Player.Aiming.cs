using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Aiming")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Vector3 parachuteCameraRotation = new Vector3(-25, 0, 0);
    [SerializeField] private ParticleSystem impactParticle;

    private float xRotation;

    private void AimUpdate()
    {
        float mouseX = 0;
        float mouseY = 0;

        if (takeInput)
        {
            mouseX = Input.GetAxis("Mouse X") * gameOptions.mouseSensitivity / 5;
            mouseY = Input.GetAxis("Mouse Y") * gameOptions.mouseSensitivity / 5;
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
        var results = new Collider[10];

        if (Physics.OverlapCapsuleNonAlloc(playerCamera.position, playerCamera.position + playerCamera.forward * 4, 0.2f, results, hitMask) > 0)
        {
            var hitInfo = results[0].gameObject;
            
            if (hitInfo.transform.TryGetComponent(out Car vehicle))
            {
                aimText.SetText(vehicle.name);
                lookingAt = hitInfo;
                return;
            }
            
            if (hitInfo.transform.TryGetComponent(out Resource resource))
            {
                aimText.SetText(resource.name);
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out InventoryGroundItem groundItem))
            {
                aimText.SetText(groundItem.name + "\n" + groundItem.amount.Value + "x");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out CraftingTable craftTable))
            {
                aimText.SetText("Crafting Table");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Oven oven))
            {
                aimText.SetText("Oven");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Animal animal))
            {
                aimText.SetText(animal.currentHealth.Value.ToString("F0") + "/" + animal.maxHealth.ToString("F0"));
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out FishingNet fishingNet))
            {
                aimText.SetText("Fishing Net" + (fishingNet.fishesInNet.Value > 0 ? $"\n{fishingNet.fishesInNet.Value}" : string.Empty));
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out ResourcePickable pickable))
            {
                aimText.SetText(pickable.gameObject.name);
                lookingAt = hitInfo;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);
    }
    
    private void EyeTraceInfo()
    {
        var invItem = hotbars[currentSlot].slot.inventoryItem;
        
        if (invItem)
        {
            switch (invItem.itemTag)
            {
                case ItemTag.Sword:
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.6f))
                    {
                        PlayToolSwing(invItem.itemTag.ToString());
                        
                        AnimatorUseSword();
                    }
                    break;
                case ItemTag.Spear:
                    AnimatorAim(InputHelper.GetKey(gameOptions.secondaryAttackKey));
                    if (InputHelper.GetKey(gameOptions.secondaryAttackKey))
                    {
                        
                        if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.3f))
                        {
                            AnimatorThrowSpear();
                        }
                        break;
                    }
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.6f))
                    {
                        PlayToolSwing(invItem.itemTag.ToString());
                        
                        AnimatorUseSpear();
                    }
                    break;
                case ItemTag.Bow:
                    AnimatorAim(InputHelper.GetKey(gameOptions.secondaryAttackKey));
                    if (!Input.GetKey(gameOptions.secondaryAttackKey)) break;
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.5f))
                    {
                        AnimatorShootBow();
                        
                        if (!lookingAt) return;
                        
                        if (lookingAt.TryGetComponent(out Animal animal))
                        {
                            animal.TakeDamageServerRpc(50, this);
                            hotbars[currentSlot].slot.Durability -= 0.01f;
                        }
                    }
                    break;
                case ItemTag.Axe:
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.6f))
                    {
                        PlayToolSwing("sword");
                        AnimatorUseAxe();
                    }
                    break;
                case ItemTag.Pickaxe:
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.6f))
                    {
                        PlayToolSwing("sword");
                        AnimatorUsePickaxe();
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
            if(InputHelper.GetKeyDown(gameOptions.useKey, 0.3f)) {
                if (resource.item.itemName == "wood")
                {
                    GiveItemServerRpc(this, "stick");
                    AnimatorCollect();
                }
            }
        }
        
        if(lookingAt.TryGetComponent(out ResourcePickable pickable))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 1f))
            {
                pickable.GatherResourceServerRpc(this);
                AnimatorCollect();
                return;
            }
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

    public void TryAttack()
    {
        if (!lookingAt) return;

        if (lookingAt.TryGetComponent(out Resource resource))
        {
            switch (hotbars[currentSlot].slot.inventoryItem.itemTag)
            {
                case ItemTag.Spear:
                case ItemTag.Sword:
                    impactParticle.Play();
                    impactParticle.Play();
                    break;
            }
        }

        if (lookingAt.TryGetComponent(out Animal animal))
        {
            switch(hotbars[currentSlot].slot.inventoryItem.itemTag)
            {
                case ItemTag.Spear:
                    animal.TakeDamageServerRpc(10, this);
                    hotbars[currentSlot].slot.Durability -= 0.05f;
                    impactParticle.Play();
                    break;
                case ItemTag.Sword:
                    animal.TakeDamageServerRpc(20, this);
                    impactParticle.Play();
                    hotbars[currentSlot].slot.Durability -= 0.025f;
                    break;
            }
        }
    }

    public void TryHarvest()
    {
        if (!lookingAt) return;
        if (lookingAt.TryGetComponent(out Resource resource))
        {
            var invItem = hotbars[currentSlot].slot.inventoryItem;
            switch (resource.item.itemName)
            {
                case "wood":
                    if(invItem.itemTag == ItemTag.Axe)
                    {
                        hotbars[currentSlot].slot.Durability -= 0.1f;
                        resource.HitResourceServerRpc(1);
                    }
                    else
                    {
                        impactParticle.Play();
                    }
                    break;
                case "stone":
                    if (invItem.itemTag == ItemTag.Pickaxe)
                    {
                        hotbars[currentSlot].slot.Durability -= 0.1f;
                        resource.HitResourceServerRpc(1);
                    }
                    else
                    {
                        impactParticle.Play();
                    }
                    break;
            }
        }
    }
    
    [ServerRpc]
    public void ThrowSpearServerRpc(NetworkBehaviourReference ply, string itemName, int slot)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            var playerCameraTransform = playerCamera.transform;
            var spear = Instantiate(weaponItems[itemName].throwablePrefab, playerCameraTransform.position + playerCameraTransform.forward,
                Quaternion.Euler(playerCameraTransform.rotation.eulerAngles + new Vector3(90, 0, 0)));
            spear.name = player.inventoryItems[itemName].name;

            spear.GetComponent<ThrowableSpear>().damage = itemName == "spear_iron" ? 30 : itemName == "spear_stone" ? 20 : 10;
            spear.GetComponent<ThrowableSpear>().player = ply;
            
            spear.AddComponent(typeof(InventoryGroundItem));
            var worldGameObjectInvItem = spear.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = player.inventoryItems[itemName];
            worldGameObjectInvItem.amount.Value = 1;
            worldGameObjectInvItem.Durability = player.hotbars[player.currentSlot].slot.Durability;

            spear.GetComponent<NetworkObject>().Spawn();
            spear.GetComponent<Rigidbody>().AddForce(playerCameraTransform.forward * 20, ForceMode.Impulse);
            
            player.hotbars[slot].slot.Clear();
        }
    }
}
