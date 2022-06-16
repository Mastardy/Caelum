using UnityEngine;
using UnityEngine.UI;

public partial class MainUI
{
    private GameOptions gameOptions;
    [SerializeField] private GameOptionsScriptableObject gameOptionsScriptableObject;

    [SerializeField] private Slider fieldOfViewSlider;
    [SerializeField] private ToggleButton compassVisibilityToggleButton;

    [SerializeField] private ToggleButton showChatToggleButton;
    [SerializeField] private ToggleButton showNameTagsToggleButton;
    [SerializeField] private ToggleButton showGameTipsToggleButton;
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider gameSoundsVolumeSlider;

    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private ToggleButton sprintToggleButton;
    [SerializeField] private ToggleButton duckToggleButton;
    [SerializeField] private ToggleButton parachuteToggleButton;
    
    // TODO: KEYBIND

    // TODO: Dropdown
    
    [SerializeField] private ToggleButton verticalSyncToggleButton;
    [SerializeField] private Slider fpsLimitSlider;
    
    /// <summary>
    /// Loads all the options from the Local Save
    /// </summary>
    private void LoadOptions()
    {
        gameOptions = SaveManager.LoadData<GameOptions>();
        gameOptions.SetHighestResolution();

        fieldOfViewSlider.value = gameOptions.fieldOfView;
        gameOptionsScriptableObject.fieldOfView = gameOptions.fieldOfView;
        compassVisibilityToggleButton.Value = gameOptions.compassVisibility;
        gameOptionsScriptableObject.compassVisibility = gameOptions.compassVisibility;

        showChatToggleButton.Value = gameOptions.showChat;
        gameOptionsScriptableObject.showChat = gameOptions.showChat;
        showNameTagsToggleButton.Value = gameOptions.showNameTags;
        gameOptionsScriptableObject.showNameTags = gameOptions.showNameTags;
        showGameTipsToggleButton.Value = gameOptions.showGameTips;
        gameOptionsScriptableObject.showGameTips = gameOptions.showGameTips;
        
        masterVolumeSlider.value = gameOptions.masterVolume * 40;
        gameOptionsScriptableObject.masterVolume = gameOptions.masterVolume;
        musicVolumeSlider.value = gameOptions.musicVolume * 20;
        gameOptionsScriptableObject.musicVolume = gameOptions.musicVolume;
        gameSoundsVolumeSlider.value = gameOptions.gameSoundsVolume;
        gameOptionsScriptableObject.gameSoundsVolume = gameOptions.gameSoundsVolume;
        voiceVolumeSlider.value = gameOptions.voiceVolume;
        gameOptionsScriptableObject.voiceVolume = gameOptions.voiceVolume;

        sensitivitySlider.value = gameOptions.mouseSensitivity;
        gameOptionsScriptableObject.mouseSensitivity = gameOptions.mouseSensitivity;
        sprintToggleButton.Value = gameOptions.toggleSprint;
        gameOptionsScriptableObject.toggleSprint = gameOptions.toggleSprint;
        duckToggleButton.Value = gameOptions.toggleDuck;
        gameOptionsScriptableObject.toggleDuck = gameOptions.toggleDuck;
        parachuteToggleButton.Value = gameOptions.toggleParachute;
        gameOptionsScriptableObject.toggleParachute = gameOptions.toggleParachute;

        verticalSyncToggleButton.Value = gameOptions.verticalSync;
        gameOptionsScriptableObject.verticalSync = gameOptions.verticalSync;
        fpsLimitSlider.value = gameOptions.fpsLimit;
        gameOptionsScriptableObject.fpsLimit = gameOptions.fpsLimit;
        
        AudioManager.Instance.UpdateVolume();
    }

    /// <summary>
    /// Saves all the options to the Local Save
    /// </summary>
    private void SaveOptions()
    {
        SaveManager.SaveData(gameOptions);
    }
}