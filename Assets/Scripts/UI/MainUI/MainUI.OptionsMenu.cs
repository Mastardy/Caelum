using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Options Menu")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject graphicsPanel;

    private GameObject currentPanel;
    
    /// <summary>
    /// Change the current Panel
    /// </summary>
    /// <param name="panel"></param>
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
    
    /// <summary>
    /// Utility responsible to toggle the ToggleButton value
    /// </summary>
    /// <param name="toggleButton"></param>
    private void ToggleButton(ToggleButton toggleButton) => toggleButton.Value = !toggleButton.Value;
}
