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
        LoadOptions();
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void CreditsMenu()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
    
    /// <summary>
    /// Quit Game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
