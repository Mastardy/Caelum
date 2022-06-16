using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Audio")]
    [SerializeField] private TextMeshProUGUI masterVolume;
    [SerializeField] private TextMeshProUGUI musicVolume;
    [SerializeField] private TextMeshProUGUI gameSoundsVolume;
    [SerializeField] private TextMeshProUGUI voiceVolume;

    /// <summary>
    /// Handles the Master Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void MasterVolumeHandle(float newValue)
    {
        newValue /= 20f;
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.1f);
        masterVolume.text = newValue.ToString("F2").Replace(",", ".");
        newValue /= 2f;
        gameOptions.masterVolume = newValue;
        gameOptionsScriptableObject.masterVolume = newValue;
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
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.2f);
        musicVolume.text = newValue.ToString("F2").Replace(",", ".");
        gameOptions.musicVolume = newValue;
        gameOptionsScriptableObject.musicVolume = newValue;
        SaveOptions();
        AudioManager.Instance.UpdateVolume();
    }
    
    /// <summary>
    /// Handles the Game SFX Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void GameSoundsVolumeHandle(float newValue)
    {
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.2f);
        gameSoundsVolume.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.gameSoundsVolume = newValue;
        gameOptionsScriptableObject.gameSoundsVolume = newValue;
        SaveOptions();
        AudioManager.Instance.UpdateVolume();
    }
    
    /// <summary>
    /// Handles the Voice Volume Value
    /// </summary>
    /// <param name="newValue"></param>
    public void VoiceVolumeHandle(float newValue)
    {
        AudioManager.Instance.PlaySoundUnsafe(sounds.uiScrollWheel, unsafeScrollWheelAudioSource, 0.2f);
        voiceVolume.text = newValue.ToString("F1").Replace(",", ".");
        gameOptions.voiceVolume = newValue;
        gameOptionsScriptableObject.voiceVolume = newValue;
        SaveOptions();
        AudioManager.Instance.UpdateVolume();
    }
}
