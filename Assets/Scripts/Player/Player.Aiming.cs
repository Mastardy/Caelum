using UnityEngine;

public partial class Player
{
    [Header("Aiming")]
    [SerializeField] private Transform headTransform;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        var headRotation = Mathf.Clamp(xRotation * 2, -45, 35f);
        
        headTransform.localRotation = Quaternion.Euler(headRotation, 0f, 0f);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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

            if (hitInfo.transform.TryGetComponent(out CraftingTable craftingTable))
            {
                aimText.SetText("Crafting Table");
                lookingAt = craftingTable.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Furnace furnace))
            {
                aimText.SetText("Furnace");
                lookingAt = furnace.gameObject;
                return;
            }

            if (hitInfo.transform.TryGetComponent(out Animal animal))
            {
                aimText.SetText(animal.currentHealth.Value + "/" + animal.maxHealth);
                lookingAt = animal.gameObject;
                return;
            }
        }
        
        lookingAt = null;
        aimText.SetText(string.Empty);
    }
    
    private void EyeTraceInfo()
    {
        if (lookingAt == null) return;

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
                switch (resource.resourceId)
                {
                    case 1:
                        resource.HitResourceServerRpc(this, 5);
                        break;
                    case 2:
                        resource.HitResourceServerRpc(this, 3);
                        break;
                    default:
                        resource.HitResourceServerRpc(this);
                        break;
                }
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
        }

        if (lookingAt.TryGetComponent(out Furnace furnace))
        {
            if (InputHelper.GetKeyDown(gameOptions.useKey, 0.1f))
            {
                furnace.CookItemServerRpc(this);
            }
        }
    }
}
