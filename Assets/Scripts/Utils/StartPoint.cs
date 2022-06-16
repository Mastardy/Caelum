using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartPoint : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private TextMeshProUGUI multiplayerText;
    
    private void Awake()
    {
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
    }

    public void PlayMultiplayer()
    {
        GetComponentInChildren<EventSystem>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
        Destroy(gameObject);
        SteamNetworkManager.Singleton.StartHost();
    }
}
