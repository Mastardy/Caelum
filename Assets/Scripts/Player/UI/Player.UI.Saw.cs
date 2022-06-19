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

    private Saw saw;

    private InventoryItem currentOutcome;
    private InventoryItem CurrentOutcome
    {
        get => currentOutcome == null ? inventoryItems["wood_plank"] : currentOutcome;
        set => currentOutcome = value;
    }

    private int outcomePrice = 2;
    
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
        hotbarsGroup.alpha = 0;
        
        playerCamera.GetComponent<Camera>().enabled = false;
        
        sawCamera.gameObject.SetActive(true);
        sawCamera.position = sawCameraPosition;
        sawCamera.rotation = Quaternion.Euler(sawCameraRotation);
        
        PrepareSaw();
    }

    public void ChangeOutcome(int outcome)
    {
        switch (outcome)
        {
            case 0:
                CurrentOutcome = inventoryItems["wood_plank"];
                outcomePrice = 2;
                break;
            case 1:
                CurrentOutcome = inventoryItems["handle_one"];
                outcomePrice = 5;
                break;
            case 2:
                CurrentOutcome = inventoryItems["handle_two"];
                outcomePrice = 7;
                break;
        }

        PrepareSaw();
    }

    private void PrepareSaw()
    {
        sawAmount.SetText(GetItemAmount("wood") + "/" + outcomePrice);
    }

    public void TrySaw()
    {
        if (saw.isSawing) return;
        if(outcomePrice > GetItemAmount("wood")) return;
        saw.SawStartServerRpc();
        if (!saw.isSawing) return;
        RemoveItem("wood", outcomePrice);
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
