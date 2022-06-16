using UnityEngine;
using UnityEngine.EventSystems;

public partial class MainUI
{
    private bool isOptionsMenu;
    
    /// <summary>
    /// Toggles Options Menu
    /// </summary>
    public void OptionsMenu()
    {   
        AudioManager.Instance.PlaySound(sounds.uiIn);
        LoadOptions();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        firstSelection.OnSubmit(new BaseEventData(EventSystem.current));
        ChangePanel(optionsPanel);
    }

    /// <summary>
    /// Play Game Menu
    /// </summary>
    public void PlayMenu()
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void CreditsMenu()
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
    
    /// <summary>
    /// Quit Game
    /// </summary>
    public void QuitGame()
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        Application.Quit();
    }
}
