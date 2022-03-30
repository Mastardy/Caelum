using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Options")] 
    [SerializeField] private TextMeshProUGUI fieldOfViewLabel;
    
    public void FieldOfView(float newValue)
    {
        fieldOfViewLabel.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.fieldOfView = newValue;
        gameOptionsScriptableObject.fieldOfView = newValue;
        SaveOptions();
    }

    public void CompassVisibility(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.compassVisibility = toggleButton.Value;
        gameOptionsScriptableObject.compassVisibility = toggleButton.Value;
        SaveOptions();
    }

    public void ShowChat(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showChat = toggleButton.Value;
        gameOptionsScriptableObject.showChat = toggleButton.Value;
        SaveOptions();
    }

    public void ShowNameTags(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showNameTags = toggleButton.Value;
        gameOptionsScriptableObject.showNameTags = toggleButton.Value;
        SaveOptions();
    }
    
    public void ShowGameTips(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showGameTips = toggleButton.Value;
        gameOptionsScriptableObject.showGameTips = toggleButton.Value;
        SaveOptions();
    }
}
