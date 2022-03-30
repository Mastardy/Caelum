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
        SaveOptions();
    }
    
    public void FramesPerSecondLimit(float newValue)
    {
        if (newValue == 0) framesPerSecondLimit.text = "INF";
        else framesPerSecondLimit.text = newValue.ToString("N0");

        gameOptions.fpsLimit = (int)newValue;
        SaveOptions();
    }
}