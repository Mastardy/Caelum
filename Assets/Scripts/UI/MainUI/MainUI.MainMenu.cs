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
}
