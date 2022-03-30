using TMPro;
using UnityEngine;

public partial class MainUI
{
    [Header("Audio")]
    [SerializeField] private TextMeshProUGUI masterVolume;
    [SerializeField] private TextMeshProUGUI musicVolume;
    [SerializeField] private TextMeshProUGUI gameSoundsVolume;
    [SerializeField] private TextMeshProUGUI voiceVolume;

    public void MasterVolume(float newValue)
    {
        masterVolume.text = newValue.ToString("F1").Replace(",", ".");
    }
    
    public void MusicVolume(float newValue)
    {
        musicVolume.text = newValue.ToString("F1").Replace(",", ".");
    }
    
    public void GameSoundsVolume(float newValue)
    {
        gameSoundsVolume.text = newValue.ToString("F1").Replace(",", ".");
    }
    
    public void VoiceVolume(float newValue)
    {
        voiceVolume.text = newValue.ToString("F1").Replace(",", ".");
    }
}
