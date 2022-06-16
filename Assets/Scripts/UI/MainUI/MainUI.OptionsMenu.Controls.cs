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
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.2f);
        mouseSensitivity.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.mouseSensitivity = newValue;
        gameOptionsScriptableObject.mouseSensitivity = newValue;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way Parachute Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleParachuteHandle(ToggleButton toggleButton)
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        ToggleButton(toggleButton);
        gameOptions.toggleParachute = toggleButton.Value;
        gameOptionsScriptableObject.toggleParachute = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way Duck Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleDuckHandle(ToggleButton toggleButton)
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
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
        AudioManager.Instance.PlaySound(sounds.uiIn);
        ToggleButton(toggleButton);
        gameOptions.toggleSprint = toggleButton.Value;
        gameOptionsScriptableObject.toggleSprint = toggleButton.Value;
        SaveOptions();
    }
}
