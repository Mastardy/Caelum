using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartPoint : Singleton<StartPoint>
{
    [SerializeField] private GameOptionsScriptableObject gameOptions;
    
    [SerializeField] private GameObject start1;
    [SerializeField] private GameObject start2;
    [SerializeField] private GameObject start3;
    
    [SerializeField] private VideoPlayer videoPlayer;
    
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private TextMeshProUGUI multiplayerText;

    public UnityEvent OnStart;
    
    protected override void Awake()
    {
        base.Awake();

        try
        {
            _ = SteamUtils.SecondsSinceAppActive;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            multiplayerButton.interactable = false;
            multiplayerText.SetText("Multiplayer (Steam not Open)");
        }
    }

    public void PlayCutscene()
    {
        start1.SetActive(false);
        start2.SetActive(true);
        
        videoPlayer.Play();
        videoPlayer.SetDirectAudioVolume(0, gameOptions.masterVolume);
        Invoke(nameof(StopCutscene), (float)videoPlayer.length + 0.5f);
    }

    public void StopCutscene()
    {
        start2.SetActive(false);
        start3.SetActive(true);
    }
    
    public void PlaySingleplayer()
    {
        GetComponentInChildren<EventSystem>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        Destroy(gameObject);
        SteamNetworkManager.Singleton.StartSingleplayer();
        
        OnStart.Invoke();
    }

    public void PlayMultiplayer()
    {
        GetComponentInChildren<EventSystem>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        Destroy(gameObject);
        SteamNetworkManager.Singleton.StartHost();
        
        OnStart.Invoke();
    }
}
