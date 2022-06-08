using TMPro;
using UnityEngine.UI;
using UnityEngine;

public partial class MainUI
{
    [Header("Options Menu")] 
    [SerializeField] private Button firstSelection;
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
        if (currentPanel == panel) return;

        if(currentPanel) currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);
        AudioManager.Instance.PlaySound(sounds.uiIn);
    }

    public void SelectButton(TextMeshProUGUI textMeshProUGUI) => textMeshProUGUI.color = new Color(0.95f, 0.8f, 0.6f);
    
    public void UnselectButton(TextMeshProUGUI textMeshProUGUI) => textMeshProUGUI.color = new Color(0.9f, 0.9f, 0.9f);
    
    /// <summary>
    /// Utility responsible to toggle the ToggleButton value
    /// </summary>
    /// <param name="toggleButton"></param>
    private void ToggleButton(ToggleButton toggleButton) => toggleButton.Value = !toggleButton.Value;
}
