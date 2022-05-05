using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public partial class Player
{
    private bool inCrafting;
    private CraftingTable craftingTable;
    
    /// <summary>
    /// Hides Crafting
    /// </summary>
    public void HideCrafting()
    {
        craftingTable = null;
        Cursor.lockState = CursorLockMode.Locked;
        inCrafting = false;
        takeInput = true;
        craftingPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Opens Crafting
    /// </summary>
    public void OpenCrafting()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inCrafting = true;
        takeInput = false;
        craftingPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
    }

    [ClientRpc]
    public void OpenCraftingClientRpc(NetworkBehaviourReference craftTable)
    {
        craftTable.TryGet(out craftingTable);
        if(craftingTable != null) OpenCrafting();
    }

    [ClientRpc]
    public void CloseCraftingClientRpc()
    {
        HideCrafting();
    }

    // public void CraftItem(string itemName)
    // {
    //     switch (itemName)
    //     {
    //         
    //     }
    // }
    
    public void CraftPickaxe(string pickaxe)
    {
        switch (pickaxe)
        {
            case "stone":
                if (GetItemAmount("stone") >= 5 && GetItemAmount("wood") >= 10)
                {
                    RemoveItem("stone", 5);
                    RemoveItem("wood", 10);
                    GiveItemClientRpc("pickaxe_stone");
                }
                break;
            case "wooden":
                if (GetItemAmount("wood") >= 15) RemoveItem("wood", 15);
                GiveItemClientRpc("pickaxe_wood");
                break;
            default:
                Debug.Log("Unknown item - trying to craft something that doesn't exist?");
                break;
        }
    }
}
