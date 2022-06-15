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
    
    private bool inSaw;
    
    /// <summary>
    /// Hides the Saw
    /// </summary>
    private void HideSaw()
    {
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

        PrepareSaw();
    }

    /// <summary>
    /// Opens the Saw
    /// </summary>
    private void OpenSaw()
    {
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
    }

    private void PrepareSaw()
    {
        sawAmount.SetText(GetItemAmount("wood").ToString("F0"));
    }

    public void TrySaw()
    {
        saw.SawStartServerRpc();
        
        // TODO: Como funciona a SAW?
    }

    private void SawUpdate()
    {
        sawTimer.SetActive(saw.isSawing);
        
        if (!saw.isSawing) return;

        sawTimerText.SetText(Mathf.CeilToInt(5 - (Time.time - saw.sawTimer)) + "sec");
        sawTimerForeground.fillAmount = (Time.time - saw.sawTimer) / 5;
    }
}
