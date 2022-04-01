using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainUI
{
    private bool isOptionsMenu;
    
    /// <summary>
    /// Toggles Options Menu
    /// </summary>
    public void ToggleOptionsMenu()
    {   
        isOptionsMenu = !isOptionsMenu;

        if (isOptionsMenu)
        {
            LoadOptions();
            optionsMenu.SetActive(true);
        }
        else
        {
            optionsMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Play Game Menu
    /// </summary>
    public void PlayGame()
    {
        LoadOptions();
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Quit Game
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit");
#else
        Application.Quit();
#endif
    }
}
