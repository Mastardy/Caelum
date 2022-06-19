using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class Player
{
    [Header("Smeltery")]
    [FormerlySerializedAs("furnacePanel")] [SerializeField] private GameObject smelteryPanel;
    [FormerlySerializedAs("furnaceCamera")] [SerializeField] private Transform smelteryCamera;
    [FormerlySerializedAs("furnaceCameraPosition")] [SerializeField] private Vector3 smelteryCameraPosition;
    [FormerlySerializedAs("furnaceCameraRotation")] [SerializeField] private Vector3 smelteryCameraRotation;

    [SerializeField] private GameObject smelteryTimer;
    [SerializeField] private TextMeshProUGUI smelteryTimerText;
    [SerializeField] private Image smelteryTimerForeground;
    
    private Smeltery smeltery;

    private string currentMineral;
    
    private bool inSmeltery;

    [SerializeField] private TMP_Dropdown mineralsDropdown;
    
    /// <summary>
    /// Hides the Furnace UI.
    /// </summary>
    private void HideSmeltery()
    {
        smeltery = null;
        
        Cursor.lockState = CursorLockMode.Locked;
        inSmeltery = false;
        takeInput = true;
        
        smelteryPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;

        playerCamera.GetComponent<Camera>().enabled = true;
        
        smelteryCamera.gameObject.SetActive(false);
        smelteryCamera.position = sawCameraPosition;
        smelteryCamera.rotation = Quaternion.Euler(sawCameraRotation);
    }
    
    /// <summary>
    /// Opens the Saw
    /// </summary>
    private void OpenSmeltery()
    {
        lookingAt.TryGetComponent(out smeltery);
        
        Cursor.lockState = CursorLockMode.Confined;
        inSmeltery = true;
        takeInput = false;
        
        smelteryPanel.SetActive(true);        
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        hotbarsGroup.alpha = 0;
        
        playerCamera.GetComponent<Camera>().enabled = false;
        
        smelteryCamera.gameObject.SetActive(true);
        smelteryCamera.position = smelteryCameraPosition;
        smelteryCamera.rotation = Quaternion.Euler(smelteryCameraRotation);

        PrepareSmeltery();
    }
    
    private void PrepareSmeltery()
    {
        mineralsDropdown.ClearOptions();
        
        var mineralOptions = new List<TMP_Dropdown.OptionData>();
        
        if(GetItemAmount("copper") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Copper", inventoryItems["copper"].sprite));
        if(GetItemAmount("iron") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Iron", inventoryItems["iron"].sprite));
        if(GetItemAmount("gold") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Gold", inventoryItems["gold"].sprite));
        
        mineralsDropdown.AddOptions(mineralOptions);
    }

    public void ChangeMineral(int mineral)
    {
        switch (mineralsDropdown.options[mineral].text)
        {
            case "Copper":
                currentMineral = "copper";
                break;
            
            case "Iron":
                currentMineral = "iron";
                break;
            
            case "Gold":
                currentMineral = "gold";
                break;
            
            default:
                currentMineral = string.Empty;
                break;
        }
    }

    public void TrySmelt()
    {
        if (smeltery.isSmelting) return;
        if(currentMineral == string.Empty) return;
        smeltery.SmeltStartServerRpc();
        if(!smeltery.isSmelting) return;

        RemoveItem(currentMineral, 1);
        GiveItemServerRpc(this, currentMineral + "_ingot", 1);

        if (GetItemAmount(currentMineral) > 0) return;
        
        PrepareSmeltery();
    }
    
    private void SmelteryUpdate()
    {
        smelteryTimer.SetActive(smeltery.isSmelting);
        
        if (!smeltery.isSmelting) return;

        smelteryTimerText.SetText(Mathf.CeilToInt(3 - (Time.time - smeltery.smelteryTimer)) + "sec");
        smelteryTimerForeground.fillAmount = (Time.time - smeltery.smelteryTimer) / 3;
    }
}
