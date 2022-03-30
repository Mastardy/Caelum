using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject graphicsPanel;

    [Header("Options")] 
    [SerializeField] private TextMeshProUGUI fieldOfViewLabel;
    
    private GameObject currentPanel;
    
    public void ChangePanel(GameObject panel)
    {
        if (currentPanel == null)
        {
            currentPanel = panel;
            currentPanel.SetActive(true);
            return;
        }
        
        if (currentPanel == panel)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
            return;
        }

        currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);
    }
    
    private void ToggleButton(ToggleButton toggleButton) => toggleButton.Value = !toggleButton.Value;
}
