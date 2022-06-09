using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public partial class Player
{
    private GameOptions gameOptionsSave;
    
    [Header("Options Menu")]
    [SerializeField] private Button firstSelection;
    [SerializeField] private GameObject optionsPanel;

    private GameObject currentPanel;
    
    /// <summary>
    /// Change the current Panel
    /// </summary>
    /// <param name="panel"></param>
    public void ChangePanel(GameObject panel)
    {
        if (currentPanel == panel) return;

        if(currentPanel) currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);
    }

    public void SelectButton(TextMeshProUGUI textMeshProUGUI) => textMeshProUGUI.color = new Color(0.95f, 0.8f, 0.6f);
    
    public void UnselectButton(TextMeshProUGUI textMeshProUGUI) => textMeshProUGUI.color = new Color(0.9f, 0.9f, 0.9f);
    
    /// <summary>
    /// Utility responsible to toggle the ToggleButton value
    /// </summary>
    /// <param name="toggleButton"></param>
    private void ToggleButton(ToggleButton toggleButton) => toggleButton.Value = !toggleButton.Value;
    
    [SerializeField] private TextMeshProUGUI masterVolume;
    [SerializeField] private TextMeshProUGUI musicVolume;
    [SerializeField] private TextMeshProUGUI gameSoundsVolume;
    [SerializeField] private TextMeshProUGUI voiceVolume;
    
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
        gameOptionsSave = SaveManager.LoadData<GameOptions>();
        gameOptionsSave.SetHighestResolution();

        fieldOfViewSlider.value = gameOptions.fieldOfView;
        gameOptionsSave.fieldOfView = gameOptions.fieldOfView;
        compassVisibilityToggleButton.Value = gameOptions.compassVisibility;
        gameOptionsSave.compassVisibility = gameOptions.compassVisibility;

        showChatToggleButton.Value = gameOptions.showChat;
        gameOptionsSave.showChat = gameOptions.showChat;
        showNameTagsToggleButton.Value = gameOptions.showNameTags;
        gameOptionsSave.showNameTags = gameOptions.showNameTags;
        showGameTipsToggleButton.Value = gameOptions.showGameTips;
        gameOptionsSave.showGameTips = gameOptions.showGameTips;
        
        masterVolumeSlider.value = gameOptions.masterVolume * 40;
        gameOptionsSave.masterVolume = gameOptions.masterVolume;
        musicVolumeSlider.value = gameOptions.musicVolume * 20;
        gameOptionsSave.musicVolume = gameOptions.musicVolume;
        gameSoundsVolumeSlider.value = gameOptions.gameSoundsVolume;
        gameOptionsSave.gameSoundsVolume = gameOptions.gameSoundsVolume;
        voiceVolumeSlider.value = gameOptions.voiceVolume;
        gameOptionsSave.voiceVolume = gameOptions.voiceVolume;

        sensitivitySlider.value = gameOptions.mouseSensitivity;
        gameOptionsSave.mouseSensitivity = gameOptions.mouseSensitivity;
        sprintToggleButton.Value = gameOptions.toggleSprint;
        gameOptionsSave.toggleSprint = gameOptions.toggleSprint;
        duckToggleButton.Value = gameOptions.toggleDuck;
        gameOptionsSave.toggleDuck = gameOptions.toggleDuck;
        parachuteToggleButton.Value = gameOptions.toggleParachute;
        gameOptionsSave.toggleParachute = gameOptions.toggleParachute;

        verticalSyncToggleButton.Value = gameOptions.verticalSync;
        gameOptionsSave.verticalSync = gameOptions.verticalSync;
        fpsLimitSlider.value = gameOptions.fpsLimit;
        gameOptionsSave.fpsLimit = gameOptions.fpsLimit;
        
        AudioManager.Instance.UpdateVolume();
    }

    /// <summary>
    /// Saves all the options to the Local Save
    /// </summary>
    private void SaveOptions()
    {
        SaveManager.SaveData(gameOptionsSave);
    }
    
    /// <summary>
    /// Handles the Master Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void MasterVolumeHandle(float newValue)
    {
        newValue /= 20f;
        masterVolume.text = newValue.ToString("F2").Replace(",", ".");
        newValue /= 2f;
        gameOptionsSave.gameSoundsVolume = newValue;
        gameOptions.masterVolume = newValue;
        SaveOptions();
        AudioManager.Instance.UpdateVolume();
    }
    
    /// <summary>
    /// Handles the Music Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void MusicVolumeHandle(float newValue)
    {
        newValue /= 20f;
        musicVolume.text = newValue.ToString("F2").Replace(",", ".");
        gameOptionsSave.gameSoundsVolume = newValue;
        gameOptions.musicVolume = newValue;
        SaveOptions();
        AudioManager.Instance.UpdateVolume();
    }
    
    /// <summary>
    /// Handles the Game SFX Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void GameSoundsVolumeHandle(float newValue)
    {
        gameSoundsVolume.text = newValue.ToString("F1").Replace(",", ".");
        gameOptionsSave.gameSoundsVolume = newValue;
        gameOptions.gameSoundsVolume = newValue;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the Voice Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void VoiceVolumeHandle(float newValue)
    {
        voiceVolume.text = newValue.ToString("F1").Replace(",", ".");
        gameOptionsSave.voiceVolume = newValue;
        gameOptions.voiceVolume = newValue;
        SaveOptions();
    }
    
    [SerializeField] private TextMeshProUGUI mouseSensitivity;
    
    /// <summary>
    /// Handles the Mouse Sensitivity value
    /// </summary>
    /// <param name="newValue"></param>
    public void MouseSensitivityHandle(float newValue)
    {
        mouseSensitivity.text = newValue.ToString("F1").Replace(",", ".");
        gameOptionsSave.mouseSensitivity = newValue;
        gameOptions.mouseSensitivity = newValue;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way Parachute Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleParachuteHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.toggleParachute = toggleButton.Value;
        gameOptions.toggleParachute = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way Duck Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleDuckHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.toggleDuck = toggleButton.Value;
        gameOptions.toggleDuck = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles the way the Sprint Input works
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ToggleSprintHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.toggleSprint = toggleButton.Value;
        gameOptions.toggleSprint = toggleButton.Value;
        SaveOptions();
    }
    
    [SerializeField] private TextMeshProUGUI framesPerSecondLimit;

    /// <summary>
    /// Handles Vertical Sync value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void VerticalSyncHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.verticalSync = toggleButton.Value;
        gameOptions.verticalSync = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles FPS Limit value
    /// </summary>
    /// <param name="newValue"></param>
    public void FramesPerSecondLimitHandle(float newValue)
    {
        framesPerSecondLimit.text = newValue == 0 ? "INF" : newValue.ToString("N0");

        gameOptionsSave.fpsLimit = (int) newValue;
        gameOptions.fpsLimit = (int) newValue;
        SaveOptions();
    }
    
    [SerializeField] private TextMeshProUGUI fieldOfViewLabel;
    
    /// <summary>
    /// Handles Field Of View value
    /// </summary>
    /// <param name="newValue"></param>
    public void FieldOfViewHandle(float newValue)
    {
        fieldOfViewLabel.text = newValue.ToString("F1").Replace(",", ".");
        gameOptionsSave.fieldOfView = newValue;
        gameOptions.fieldOfView = newValue;
        SaveOptions();
        playerCamera.GetComponent<Camera>().fieldOfView = 60 + (gameOptions.fieldOfView - 90f) * 0.875f;
        weaponCamera.GetComponent<Camera>().fieldOfView = 60 + (gameOptions.fieldOfView - 90f) * 0.875f;
    }

    /// <summary>
    /// Handles Compass Visibility value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void CompassVisibilityHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.compassVisibility = toggleButton.Value;
        gameOptions.compassVisibility = toggleButton.Value;
        SaveOptions();
    }

    /// <summary>
    /// Handles Show Chat value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowChatHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.showChat = toggleButton.Value;
        gameOptions.showChat = toggleButton.Value;
        SaveOptions();
    }

    /// <summary>
    /// Handles Show Name Tags value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowNameTagsHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.showNameTags = toggleButton.Value;
        gameOptions.showNameTags = toggleButton.Value;
        SaveOptions();
    }
    
    /// <summary>
    /// Handles Show Game Tips value
    /// </summary>
    /// <param name="toggleButton"></param>
    public void ShowGameTipsHandle(ToggleButton toggleButton)
    {
        ToggleButton(toggleButton);
        gameOptionsSave.showGameTips = toggleButton.Value;
        gameOptions.showGameTips = toggleButton.Value;
        SaveOptions();
    }
}
