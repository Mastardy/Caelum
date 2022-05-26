using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartPoint : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private TextMeshProUGUI multiplayerText;
    
    private void Awake()
    {
        try
        {
            var seconds = SteamUtils.SecondsSinceAppActive;
        }
        catch (Exception e)
        {
            multiplayerButton.interactable = false;
            multiplayerText.SetText("Multiplayer (Steam not Open)");
        }
    }

    public void PlaySingleplayer()
    {
        SteamNetworkManager.Singleton.StartSingleplayer();
        Destroy(gameObject);
    }

    public void PlayMultiplayer()
    {
        SteamNetworkManager.Singleton.StartHost();
        Destroy(gameObject);
    }
}
