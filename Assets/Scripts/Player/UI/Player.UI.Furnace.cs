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
    /// Hides the Furnace UI.
    /// </summary>
    private void HideFurnace()
    {
        furnace = null;
        
        Cursor.lockState = CursorLockMode.Locked;
        inFurnace = false;
        takeInput = true;
        
        furnacePanel.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
        hotbarsGroup.alpha = 1;

        playerCamera.GetComponent<Camera>().enabled = true;
        
        furnaceCamera.gameObject.SetActive(false);
        furnaceCamera.position = sawCameraPosition;
        furnaceCamera.rotation = Quaternion.Euler(sawCameraRotation);
    }
    
    /// <summary>
    /// Opens the Saw
    /// </summary>
    private void OpenFurnace()
    {
        lookingAt.TryGetComponent(out furnace);
        
        Cursor.lockState = CursorLockMode.Confined;
        inFurnace = true;
        takeInput = false;
        
        furnacePanel.SetActive(true);        
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
        hotbarsGroup.alpha = 0;
        
        playerCamera.GetComponent<Camera>().enabled = false;
        
        furnaceCamera.gameObject.SetActive(true);
        furnaceCamera.position = furnaceCameraPosition;
        furnaceCamera.rotation = Quaternion.Euler(furnaceCameraRotation);

        PrepareFurnace();
    }
    
    private void PrepareFurnace()
    {
        
    }

    private void FurnaceUpdate()
    {
        
    }
}
