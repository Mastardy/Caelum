using Unity.Netcode;
using UnityEngine;

public partial class Player
{
    [Header("Aiming")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Vector3 parachuteCameraRotation = new Vector3(-25, 0, 0);
    [SerializeField] private ParticleSystem impactParticle;
    [SerializeField] private GameObject stoneKnife;

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
                tipsText.SetText("[E] Enter vechicle");
                lookingAt = hitInfo;
                return;
            }
            
            if (hitInfo.transform.TryGetComponent(out Resource resource))
            {
                aimText.SetText(resource.name);
                lookingAt = hitInfo;

                if (resource.item.itemName == "wood")
                {
                    if (!hotbars[currentSlot].slot.isEmpty)
                    {
                        if (hotbars[currentSlot].slot.inventoryItem.itemTag is ItemTag.Axe) tipsText.SetText("[E] Chop\n[LMB] Collect stick");
                        else tipsText.SetText("[E] Collect stick");
                    }
                    else tipsText.SetText("[E] Collect stick");
                }
                else if(resource.item.itemName == "stone")
                {
                    if (!hotbars[currentSlot].slot.isEmpty)
                    {
                        if (hotbars[currentSlot].slot.inventoryItem.itemTag is ItemTag.Pickaxe) tipsText.SetText("[LMB] Mine");
                        else tipsText.SetText(string.Empty);
                    }
                    else tipsText.SetText(string.Empty);
                }
                    
                
                return;
            }

            if (hitInfo.transform.TryGetComponent(out InventoryGroundItem groundItem))
            {
                aimText.SetText(groundItem.inventoryItem.displayName + "\n" + groundItem.amount.Value + "x");
                tipsText.SetText("[E] Collect");

                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out CraftingTable _))
            {
                aimText.SetText("Crafting Table");
                tipsText.SetText("[E] Open menu");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Oven _))
            {
                aimText.SetText("Oven");
                tipsText.SetText("[E] Open menu");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out AnimalBone animalbone))
            {
                Animal animal = animalbone.animalOwner;
                aimText.SetText(animal.dead? string.Empty : animal.currentHealth.Value.ToString("F0") + "/" + animal.maxHealth.ToString("F0"));
                if(animal.dead) tipsText.SetText("[E] Carve");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out FishingNet fishingNet))
            {
                aimText.SetText("Fishing Net" + (fishingNet.fishesInNet.Value > 0 ? $"\n{fishingNet.fishesInNet.Value}" : string.Empty));
                tipsText.SetText("[E] Use");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out ResourcePickable pickable))
            {
                aimText.SetText(pickable.gameObject.name);
                tipsText.SetText("[E] Collect");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Saw _))
            {
                aimText.SetText("Saw");
                tipsText.SetText("[E] Open menu");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Smeltery _))
            {
                aimText.SetText("Smeltery");
                tipsText.SetText("[E] Open menu");
                lookingAt = hitInfo;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out CropField cf))
            {
                aimText.SetText(cf.hasSeed ? cf.cropName + "\n" + ((int)(cf.seeds[0].GetComponent<Crop>().percentage * 100)).ToString() + "%" : "Crop Field");
                if(cf.harvestable.Value) tipsText.SetText("[E] Collect");
                else
                {
                    if (!hotbars[CurrentSlot].slot.isEmpty)
                    {
                        if(hotbars[CurrentSlot].slot.inventoryItem.subTag == SubTag.Seed) tipsText.SetText("[E] Plant seed");
                        else tipsText.SetText(string.Empty);
                    }
                    else tipsText.SetText(string.Empty);
                }

                lookingAt = hitInfo;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);

        if (!hotbars[CurrentSlot].slot.isEmpty)
        {
            switch (hotbars[CurrentSlot].slot.inventoryItem.itemTag)
            {
                case ItemTag.Bow:
                    if(animAim) tipsText.SetText("[LMB] Shoot");
                    else tipsText.SetText("[RMB] Aim (hold)");
                    break;
                case ItemTag.Sword:
                    tipsText.SetText("[LMB] Attack");
                    break;
                case ItemTag.Food:
                    tipsText.SetText("[LMB] Eat");
                    break;
                case ItemTag.Grappling:
                    tipsText.SetText("[RMB] Shoot");
                    break;
                case ItemTag.Spear:
                    if(animAim) tipsText.SetText("[LMB] Throw");
                    else tipsText.SetText("[RMB] Aim (hold)\n[LMB] Attack");
                    break;
            }
            return;
        }
        
        tipsText.SetText(string.Empty);
    }

    private bool lastBowState;
    
    private void EyeTraceInfo()
    {
        var invItem = hotbars[currentSlot].slot.inventoryItem;
        
        if (invItem && !inParachute)
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
                        if (Physics.Raycast(playerCamera.position, playerCamera.forward, 2, groundMask)) break;
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
                    if (!Input.GetKey(gameOptions.secondaryAttackKey))
                    {
                        lastBowState = false;
                        break;
                    }
                    if (!currentArrow) SetBowArrow();
                    if(!lastBowState) AudioManager.Instance.PlaySound(sounds.bowPull);
                    lastBowState = true;
                    if (Physics.Raycast(playerCamera.position, playerCamera.forward, 2, groundMask)) break;
                    if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.5f))
                    {
                        if (!currentArrow) return;
                        AnimatorShootBow();
                        AudioManager.Instance.PlaySound(sounds.bowShoot);
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
                case ItemTag.Food:
                    if (invItem.subTag is SubTag.Drink or SubTag.Food)
                    {

                        if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.15f))
                        {
                            EatOrDrink(hotbars[currentSlot].slot);
                        }
                    }
                    break;
                case ItemTag.Other:
                    if(invItem.subTag is SubTag.Seed)
                    {
                        if (InputHelper.GetKeyDown(gameOptions.useKey, 0.5f))
                        {
                            if (!lookingAt) return;
                            if (lookingAt.TryGetComponent(out CropField cropField))
                            {
                                cropField.PlantSeedsServerRpc(this, invItem.itemName);
                            }
                        }
                    }
                    break;
                case ItemTag.Grappling:
                    if(InputHelper.GetKeyDown(gameOptions.secondaryAttackKey, 0.6f))
                    {
                        if(!isTethered && !isTetheredPlus)
                        {
                            BeginGrapple();
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
            if(InputHelper.GetKeyDown(gameOptions.useKey, 0.8f)) {
                if (resource.item.itemName == "wood")
                {
                    GiveItemServerRpc(this, "stick");
                    AnimatorCollect();
                    return;
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

            return;
        }
        
        if(lookingAt.TryGetComponent(out Saw _))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.3f))
            {
                OpenSaw();
            }

            return;
        }

        if (lookingAt.TryGetComponent(out Smeltery _))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.3f))
            {
                OpenSmeltery();
            }

            return;
        }

        if (lookingAt.TryGetComponent(out CropField cropfield))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.3f))
            {
                if(cropfield.harvestable.Value)
                {
                    cropfield.HarvestServerRpc(this);
                    AnimatorCollect();
                }
            }
            return;
        }

        if (lookingAt.TryGetComponent(out AnimalBone animalbone))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 1f))
            {              
                if(animalbone.animalOwner.carved.Value == false) AnimatorCarve();
                animalbone.animalOwner.CarveServerRpc(this);
            }

            return;
        }
    }

    public void TryAttack()
    {
        if (!lookingAt) return;

        if (lookingAt.TryGetComponent(out Resource _))
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

        if (lookingAt.TryGetComponent(out AnimalBone animalbone))
        {
            Animal animal = animalbone.animalOwner;

            if (animal.dead) return;

            switch(hotbars[currentSlot].slot.inventoryItem.itemTag)
            {
                case ItemTag.Spear:
                    animal.TakeDamageServerRpc(10);
                    hotbars[currentSlot].slot.Durability -= 0.05f;
                    impactParticle.Play();
                    break;
                case ItemTag.Sword:
                    animal.TakeDamageServerRpc(20);
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
                        resource.HitResourceServerRpc();
                        AudioManager.Instance.PlaySound(sounds.axeHit);
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
                        resource.HitResourceServerRpc();
                        AudioManager.Instance.PlaySound(sounds.pickaxeHit);
                    }
                    else
                    {
                        impactParticle.Play();
                    }
                    break;
            }
        }
    }

    public void ShowStoneKnife(bool show)
    {
        stoneKnife.SetActive(show);
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

            spear.GetComponent<Throwable>().damage = itemName == "spear_iron" ? 30 : itemName == "spear_stone" ? 20 : 10;
            spear.GetComponent<Throwable>().player = ply;
            
            spear.AddComponent(typeof(InventoryGroundItem));
            var worldGameObjectInvItem = spear.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = player.inventoryItems[itemName];
            worldGameObjectInvItem.amount.Value = 1;
            worldGameObjectInvItem.Durability = player.hotbars[player.currentSlot].slot.Durability;

            spear.GetComponent<NetworkObject>().Spawn();
            spear.GetComponent<Rigidbody>().AddForce(playerCameraTransform.forward * 30, ForceMode.Impulse);
            
            player.hotbars[slot].slot.Clear();
        }
    }

    [ServerRpc]
    public void ThrowArrowServerRpc(NetworkBehaviourReference ply, string itemName)
    {
        if (!IsServer) return;

        if (ply.TryGet(out Player player))
        {
            var playerCameraTransform = playerCamera.transform;
            var arrow = Instantiate(weaponItems[itemName].throwablePrefab,
                playerCameraTransform.position + playerCameraTransform.forward,
                Quaternion.Euler(playerCameraTransform.rotation.eulerAngles + new Vector3(90, 0, 0)));
            arrow.name = player.inventoryItems[itemName].name;

            arrow.GetComponent<Throwable>().damage = itemName == "arrow_iron" ? 30 : itemName == "spear_stone" ? 20 : 10;
            arrow.GetComponent<Throwable>().player = ply;

            arrow.AddComponent(typeof(InventoryGroundItem));
            var worldGameObjectInvItem = arrow.GetComponent<InventoryGroundItem>();
            worldGameObjectInvItem.inventoryItem = player.inventoryItems[itemName];
            worldGameObjectInvItem.amount.Value = 1;

            arrow.GetComponent<NetworkObject>().Spawn();
            arrow.GetComponent<Rigidbody>().AddForce(playerCameraTransform.forward * 40, ForceMode.Impulse);
            
            player.RemoveItem(itemName, 1);
            player.SetBowArrow();
        }
    }
}
