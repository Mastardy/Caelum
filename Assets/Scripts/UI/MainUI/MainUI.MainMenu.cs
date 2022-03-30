using UnityEngine;

public partial class MainUI
{
    private bool isOptionsMenu;
    
    public void ToggleOptionsMenu()
    {   
        isOptionsMenu = !isOptionsMenu;

        if (isOptionsMenu)
        {
            optionsMenu.SetActive(true);
        }
        else
        {
            optionsMenu.SetActive(false);
        }
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
