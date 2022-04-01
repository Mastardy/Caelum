using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Options")] 
    [SerializeField] private TextMeshProUGUI fieldOfViewLabel;
    
    /// <summary>
    /// Handles Field Of View value
    /// </summary>
    /// <param name="newValue"></param>
    public void FieldOfViewHandle(float newValue)
    {
        fieldOfViewLabel.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.fieldOfView = newValue;
        gameOptionsScriptableObject.fieldOfView = newValue;
        SaveOptions();
    }

    /// <summary>
    /// Handles Compass Visibility value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void CompassVisibilityHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.compassVisibility = toggleButton.Value;
        gameOptionsScriptableObject.compassVisibility = toggleButton.Value;
        SaveOptions();
    }

    /// <summary>
    /// Handles Show Chat value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowChatHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showChat = toggleButton.Value;
        gameOptionsScriptableObject.showChat = toggleButton.Value;
        SaveOptions();
    }

    /// <summary>
    /// Handles Show Name Tags value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowNameTagsHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showNameTags = toggleButton.Value;
        gameOptionsScriptableObject.showNameTags = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles Show Game Tips value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowGameTipsHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.showGameTips = toggleButton.Value;
        gameOptionsScriptableObject.showGameTips = toggleButton.Value;
        SaveOptions();
    }
}
