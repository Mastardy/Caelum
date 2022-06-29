using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartPoint : Singleton<StartPoint>
{
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
