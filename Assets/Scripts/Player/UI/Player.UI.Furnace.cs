using UnityEngine;

public partial class Player
{
    [Header("Furnace")]
    [SerializeField] private GameObject furnacePanel;
    [SerializeField] private Transform furnaceCamera;
    [SerializeField] private Vector3 furnaceCameraPosition;
    [SerializeField] private Vector3 furnaceCameraRotation;

    private Furnace furnace;
    
    private bool inFurnace;
    
    /// <summary>
    /// Hides the Saw
    /// </summary>
    private void HideFurnace()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inFurnace = false;
        takeInput = true;
        
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;

        playerCamera.GetComponent<Camera>().enabled = true;
    }
    
    /// <summary>
    /// Opens the Saw
    /// </summary>
    private void OpenFurnace()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inFurnace = true;
        takeInput = false;
        
        
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        hotbarsGroup.alpha = 0;
        
        playerCamera.GetComponent<Camera>().enabled = false;
    }
}
