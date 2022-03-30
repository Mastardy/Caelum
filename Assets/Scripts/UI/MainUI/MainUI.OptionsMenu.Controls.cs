using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Controls")]
    [SerializeField] private TextMeshProUGUI mouseSensitivity;
    
    public void MouseSensitivity(float newValue)
    {
        mouseSensitivity.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.mouseSensitivity = newValue;
        gameOptionsScriptableObject.mouseSensitivity = newValue;
        SaveOptions();
    }
    
    public void ToggleDuck(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.toggleDuck = toggleButton.Value;
        gameOptionsScriptableObject.toggleDuck = toggleButton.Value;
        SaveOptions();
    }
    
    public void ToggleSprint(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.toggleSprint = toggleButton.Value;
        gameOptionsScriptableObject.toggleSprint = toggleButton.Value;
        SaveOptions();
    }
}
