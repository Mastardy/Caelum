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
    [SerializeField] private Image smelteryCostSprite;
    [SerializeField] private TextMeshProUGUI smelteryAmount;

    private Smeltery smeltery;

    private string currentMineral;
    private string currentMold;
    private int smelteryOutcomePrice;
    private string lastCurrentSmelterOutcome;


    private bool inSmeltery;

    [SerializeField] private TMP_Dropdown mineralsDropdown;
    
    private void HideSmeltery()
    {
        smeltery = null;
        
        Cursor.lockState = CursorLockMode.Locked;
        inSmeltery = false;
        takeInput = true;
        
        smelteryPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        tipsText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;

        playerCamera.GetComponent<Camera>().enabled = true;
        
        smelteryCamera.gameObject.SetActive(false);
        smelteryCamera.position = sawCameraPosition;
        smelteryCamera.rotation = Quaternion.Euler(sawCameraRotation);
    }
    
    private void OpenSmeltery()
    {
        lookingAt.TryGetComponent(out smeltery);
        
        Cursor.lockState = CursorLockMode.Confined;
        inSmeltery = true;
        takeInput = false;
        
        smelteryPanel.SetActive(true);        
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        tipsText.gameObject.SetActive(false);
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

        if (GetItemAmount("pickaxe_mold") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Pickaxe Mold", inventoryItems["pickaxe_mold"].sprite));
        if (GetItemAmount("axe_mold") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Axe Mold", inventoryItems["axe_mold"].sprite));
        if (GetItemAmount("sword_mold") > 0) mineralOptions.Add(new TMP_Dropdown.OptionData("Sword Mold", inventoryItems["sword_mold"].sprite));

        mineralsDropdown.AddOptions(mineralOptions);

        ChangeMineral(0);
    }

    public void ChangeMineral(int mineral)
    {
        if(mineralsDropdown.options.Count == 0) return;
        
        switch (mineralsDropdown.options[mineral].text)
        {
            case "Pickaxe Mold":
                currentMold = "pickaxe";
                currentMineral = "iron";
                smelteryOutcomePrice = 1;
                break;

            case "Axe Mold":
                currentMold = "axe";
                currentMineral = "iron";
                smelteryOutcomePrice = 1;
                break;

            case "Sword Mold":
                currentMold = "sword";
                currentMineral = "iron";
                smelteryOutcomePrice = 1;
                break;

            default:
                currentMineral = string.Empty;
                currentMold = string.Empty;
                break;
        }

        smelteryAmount.SetText(GetItemAmount(currentMineral) + "/" + smelteryOutcomePrice);
        smelteryCostSprite.sprite = inventoryItems[currentMineral].sprite;
    }

    public void TrySmelt()
    {
        if (smeltery.isSmelting) return;
        if(currentMineral == string.Empty && currentMold == string.Empty) return;
        if (smelteryOutcomePrice > GetItemAmount(currentMineral)) return;
        smeltery.SmeltStartServerRpc();
        if(!smeltery.isSmelting) return;

        RemoveItem(currentMineral, smelteryOutcomePrice);
        lastCurrentSmelterOutcome = currentMold + "_blade";
        AudioManager.Instance.PlaySound(sounds.smeltery);
        Invoke("GiveSmelterOutcome", 3f);

        smelteryAmount.SetText(GetItemAmount(currentMineral) + "/" + smelteryOutcomePrice);
        smelteryCostSprite.sprite = inventoryItems[currentMineral].sprite;

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

    private void GiveSmelterOutcome()
    {
        GiveItemServerRpc(this, lastCurrentSmelterOutcome, 1);
    }
}
