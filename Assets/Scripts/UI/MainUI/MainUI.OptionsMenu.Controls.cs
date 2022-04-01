using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Controls")]
    [SerializeField] private TextMeshProUGUI mouseSensitivity;
    
    /// <summary>
    /// Handles the Mouse Sensitivity value
    /// </summary>
    /// <param name="newValue"></param>
    public void MouseSensitivityHandle(float newValue)
    {
        mouseSensitivity.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.mouseSensitivity = newValue;
        gameOptionsScriptableObject.mouseSensitivity = newValue;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way Duck Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleDuckHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.toggleDuck = toggleButton.Value;
        gameOptionsScriptableObject.toggleDuck = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way the Sprint Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleSprintHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.toggleSprint = toggleButton.Value;
        gameOptionsScriptableObject.toggleSprint = toggleButton.Value;
        SaveOptions();
    }
}
