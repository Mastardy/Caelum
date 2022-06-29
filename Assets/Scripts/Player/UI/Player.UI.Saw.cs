using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player
{
    [Header("Saw")]
    [SerializeField] private GameObject sawPanel;
    [SerializeField] private Transform sawCamera;
    [SerializeField] private Vector3 sawCameraPosition;
    [SerializeField] private Vector3 sawCameraRotation;
    [SerializeField] private GameObject sawTimer;
    [SerializeField] private TextMeshProUGUI sawTimerText;
    [SerializeField] private Image sawTimerForeground;
    [SerializeField] private TextMeshProUGUI sawAmount;
    [SerializeField] private Image sawCostSprite;
    private string sawCostMaterial;

    private Saw saw;

    private InventoryItem currentOutcome;
    private InventoryItem CurrentOutcome
    {
        get => currentOutcome == null ? inventoryItems["wood_plank"] : currentOutcome;
        set => currentOutcome = value;
    }

    private int sawOutcomePrice = 2;
    
    private bool inSaw;
    
    /// <summary>
    /// Hides the Saw
    /// </summary>
    private void HideSaw()
    {
        saw = null;
        
        Cursor.lockState = CursorLockMode.Locked;
        inSaw = false;
        takeInput = true;
        sawPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        tipsText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;

        playerCamera.GetComponent<Camera>().enabled = true;
        
        sawCamera.gameObject.SetActive(false);
        sawCamera.position = sawCameraPosition;
        sawCamera.rotation = Quaternion.Euler(sawCameraRotation);
    }

    /// <summary>
    /// Opens the Saw
    /// </summary>
    private void OpenSaw()
    {
        lookingAt.TryGetComponent(out saw);
        
        Cursor.lockState = CursorLockMode.Confined;
        inSaw = true;
        takeInput = false;
        sawPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        tipsText.gameObject.SetActive(false);
        hotbarsGroup.alpha = 0;
        
        playerCamera.GetComponent<Camera>().enabled = false;
        
        sawCamera.gameObject.SetActive(true);
        sawCamera.position = sawCameraPosition;
        sawCamera.rotation = Quaternion.Euler(sawCameraRotation);
        
        PrepareSaw();
        ChangeOutcome(0);
    }

    public void ChangeOutcome(int outcome)
    {
        switch (outcome)
        {
            case 0:
                CurrentOutcome = inventoryItems["wood"];
                sawOutcomePrice = 4;
                sawCostMaterial = "stick";
                break;
            case 1:
                CurrentOutcome = inventoryItems["wood_plank"];
                sawOutcomePrice = 2;
                sawCostMaterial = "wood";
                break;
            case 2:
                CurrentOutcome = inventoryItems["handle_two"];
                sawOutcomePrice = 2;
                sawCostMaterial = "wood";
                break;
            case 3:
                CurrentOutcome = inventoryItems["handle_one"];
                sawOutcomePrice = 1;
                sawCostMaterial = "wood";
                break;
        }
        sawCostSprite.sprite = inventoryItems[sawCostMaterial].sprite;

        PrepareSaw();
    }

    private void PrepareSaw()
    {
        sawAmount.SetText(GetItemAmount(sawCostMaterial) + "/" + sawOutcomePrice);
    }

    public void TrySaw()
    {
        if (saw.isSawing) return;
        if(sawOutcomePrice > GetItemAmount(sawCostMaterial)) return;
        saw.SawStartServerRpc();
        if (!saw.isSawing) return;
        RemoveItem(sawCostMaterial, sawOutcomePrice);
        GiveItemServerRpc(this, CurrentOutcome.itemName);
        PrepareSaw();
    }

    private void SawUpdate()
    {
        sawTimer.SetActive(saw.isSawing);
        
        if (!saw.isSawing) return;

        sawTimerText.SetText(Mathf.CeilToInt(5 - (Time.time - saw.sawTimer)) + "sec");
        sawTimerForeground.fillAmount = (Time.time - saw.sawTimer) / 5;
    }
}
