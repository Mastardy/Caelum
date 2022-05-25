using UnityEngine;

public partial class MainUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;

    private void Awake()
    {
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        
        optionsMenu.SetActive(false);
        playMenu.SetActive(false);
        mainMenu.SetActive(true);
        
        LoadOptions();
    }

    public void MainMenu()
    {
        optionsMenu.SetActive(false);
        playMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
