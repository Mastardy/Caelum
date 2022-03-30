using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainUI
{
    private bool isOptionsMenu;
    
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

    public void PlayGame()
    {
        LoadOptions();
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit");
#else
        Application.Quit();
#endif
    }
}
