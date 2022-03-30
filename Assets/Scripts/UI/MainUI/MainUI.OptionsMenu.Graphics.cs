using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Graphics")]
    [SerializeField] private TextMeshProUGUI framesPerSecondLimit;

    public void VerticalSync(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptions.verticalSync = toggleButton.Value;
        gameOptionsScriptableObject.verticalSync = toggleButton.Value;
        SaveOptions();
    }
    
    public void FramesPerSecondLimit(float newValue)
    {
        framesPerSecondLimit.text = newValue == 0 ? "INF" : newValue.ToString("N0");

        gameOptions.fpsLimit = (int) newValue;
        gameOptionsScriptableObject.fpsLimit = (int) newValue;
        SaveOptions();
    }
}