using UnityEngine;

public partial class MainUI : MonoBehaviour
{
    [SerializeField] private SoundScriptableObject sounds;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;

    private int unsafeScrollWheelAudioSource;
    
    private void Awake()
    {
        unsafeScrollWheelAudioSource = AudioManager.Instance.CreateUnsafeAudioSource();
        
        optionsPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        graphicsPanel.SetActive(false);
        
        playMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
        
        LoadOptions();
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySound(sounds.uiOut);
        playMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
