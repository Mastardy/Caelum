using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.EventSystems;

public partial class Player
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
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
        optionsMenu.SetActive(false);
        crosshair.SetActive(true);
        aimText.gameObject.SetActive(true);
    }

    public void Restart()
    {
        RespawnPlayer(spawnPosition, false);
        HidePauseMenu();
    }

    public void OptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        LoadOptions();
        firstSelection.OnSubmit(new BaseEventData(EventSystem.current));
        ChangePanel(optionsPanel);
    }

    public void BackPauseMenu()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
    
    public void MainMenu()
    {
        SteamNetworkManager.Singleton.Disconnect();
        if(NetworkManager.Singleton.ShutdownInProgress) NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene(0);
    }
}
