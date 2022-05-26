using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public partial class Player
{
    [SerializeField] private GameObject pauseMenu;
    private bool inPause;
    
    public void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inPause = true;
        takeInput = false;
        pauseMenu.SetActive(true);
        crosshair.SetActive(false);
        aimText.gameObject.SetActive(false);
    }

    public void HidePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inPause = false;
        takeInput = true;
        pauseMenu.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    public void Restart()
    {
        RespawnPlayer(false);
    }
    
    public void MainMenu()
    {
        SteamNetworkManager.Singleton.Disconnect();
        if(NetworkManager.Singleton.ShutdownInProgress) NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene(0);
    }
}
