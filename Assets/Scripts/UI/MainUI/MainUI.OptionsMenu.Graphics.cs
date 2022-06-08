using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Graphics")]
    [SerializeField] private TextMeshProUGUI framesPerSecondLimit;

    /// <summary>
    /// Handles Vertical Sync value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void VerticalSyncHandle(ToggleButton toggleButton)
    {
        AudioManager.Instance.PlaySound(sounds.uiIn);
        ToggleButton(toggleButton);
        gameOptions.verticalSync = toggleButton.Value;
        gameOptionsScriptableObject.verticalSync = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles FPS Limit value
    /// </summary>
    /// <param name="newValue"></param>
    public void FramesPerSecondLimitHandle(float newValue)
    {
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.2f);
        framesPerSecondLimit.text = newValue == 0 ? "INF" : newValue.ToString("N0");
        gameOptions.fpsLimit = (int) newValue;
        gameOptionsScriptableObject.fpsLimit = (int) newValue;
        SaveOptions();
    }
}