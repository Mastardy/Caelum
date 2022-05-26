using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player
{
    [SerializeField] private GameObject pauseMenu;
    private bool inPause;
    
    public void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inPause = true;
        takeInput = false;
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
    }

    public void HidePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inPause = false;
        takeInput = true;
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    public void Restart()
    {
        RespawnPlayer(false);
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
