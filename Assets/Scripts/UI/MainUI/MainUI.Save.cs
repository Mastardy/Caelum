using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class MainUI
{
    private GameOptions gameOptions;

    [SerializeField] private Slider fieldOfViewSlider;
    [SerializeField] private ToggleButton compassVisibilityToggleButton;

    [SerializeField] private ToggleButton showChatToggleButton;
    [SerializeField] private ToggleButton showNameTagsToggleButton;
    [SerializeField] private ToggleButton showGameTipsToggleButton;
    
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider gameSoundsVolumeSlider;
    
    // TODO: KEYBIND

    
    // TODO: Dropdown
    
    [SerializeField] private ToggleButton verticalSyncToggleButton;
    [SerializeField] private Slider fpsLimitSlider;
    
    private void LoadOptions()
    {
        gameOptions = SaveManager.LoadData<GameOptions>();
        gameOptions.SetHighestResolution();

        fieldOfViewSlider.value = gameOptions.fieldOfView;
        compassVisibilityToggleButton.Value = gameOptions.compassVisibility;

        showChatToggleButton.Value = gameOptions.showChat;
        showNameTagsToggleButton.Value = gameOptions.showNameTags;
        showGameTipsToggleButton.Value = gameOptions.showGameTips;
        
        masterVolumeSlider.value = gameOptions.masterVolume;
        musicVolumeSlider.value = gameOptions.musicVolume;
        gameSoundsVolumeSlider.value = gameOptions.gameSoundsVolume;
        voiceVolumeSlider.value = gameOptions.voiceVolume;

        verticalSyncToggleButton.Value = gameOptions.verticalSync;
        fpsLimitSlider.value = gameOptions.fpsLimit;
    }

    private void SaveOptions()
    {
        SaveManager.SaveData(gameOptions);
    }
}