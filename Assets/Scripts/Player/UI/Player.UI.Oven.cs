using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public partial class Player
{
    private int itemId;
    private bool inOven;
    private Oven cooker;

    private Dictionary<string, Dictionary<string, int>> cookItems = new();

    /// <summary>
    /// Opens Oven
    /// </summary>
    public void OpenOven()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inOven = true;
        takeInput = false;
        ovenPanel.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        
        PrepareOven();
    }

    /// <summary>
    /// Hides Oven
    /// </summary>
    public void HideOven()
    {
        cooker = null;
        Cursor.lockState = CursorLockMode.Locked;
        inOven = false;
        takeInput = true;
        ovenPanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void OpenOvenClientRpc(NetworkBehaviourReference cookerReference)
    {
        cookerReference.TryGet(out cooker);
        if(cooker != null) OpenOven();
    }

    [ClientRpc]
    public void CloseOvenClientRpc()
    {
        HideOven();
    }

    private float currentTimer;
    
    private void PrepareOven()
    {
        currentTimer = 0.0f;
        int randomValue = Random.Range(50, 90);
        ovenScaler.localScale = new Vector2(0, 1);
        ovenArrow.anchoredPosition = new Vector2(randomValue * 4, 0);
    }
    
    private void OvenUpdate()
    {
        currentTimer += Time.deltaTime / ovenSpeed;
        var currentTimerFloor = Mathf.FloorToInt(currentTimer);
        var tempCurrentTimer = currentTimer - currentTimerFloor;

        if (currentTimerFloor % 2 == 1) tempCurrentTimer = 1 - tempCurrentTimer;

        ovenScaler.localScale = new Vector2(tempCurrentTimer, 1);

        if (InputHelper.GetKeyDown(gameOptions.primaryAttackKey, 0.1f))
        {
            var curValue = ovenArrow.anchoredPosition.x / 400f;
            var curOffset = ovenArrow.rect.width / 800f; 
            Debug.Log(curValue > (tempCurrentTimer - curOffset) && curValue < (tempCurrentTimer + curOffset) ? "Acertou" : "Errou");
        }
    }
}
