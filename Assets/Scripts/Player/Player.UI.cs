using System;
using System.Threading.Tasks;
using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public partial class Player
{
    [Header("UI")]
    [SerializeField] private Canvas playerCanvas;

    [SerializeField] private Transform chatPanel;
    [SerializeField] private GameObject chatEntryPrefab;
    
    [SerializeField] private TextMeshProUGUI aimText;
    [SerializeField] private LayerMask resourceMask;

    [SerializeField] private GameObject chatBox;
    
    [HideInInspector] public GameObject lookingAt;
    
    [SerializeField] private TextMeshProUGUI woodText;
    private int wood;
    public int Wood
    {
        get => wood;
        set
        {
            woodText.SetText(value.ToString());
            wood = value;
        }
    }
    
    [SerializeField] private TextMeshProUGUI stoneText;
    private int stone;
    private int Stone
    {
        get => stone;
        set
        {
            stoneText.SetText(value.ToString());
            stone = value;
        }
    }
    
    [ClientRpc]
    public void CreateChatEntryClientRpc(string message, string clientName, SteamId client)
    {
        Debug.Log("Teste");
        
        var chatEntry = Instantiate(chatEntryPrefab, chatPanel);

        chatEntry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=#55FF55>{clientName}:</color> {message}";

        var avatar = GetAvatar(client).Result;

        if (avatar == null) return;
        
        chatEntry.GetComponentInChildren<Image>().sprite = Sprite.Create(Covert(avatar.Value), new Rect(0.0f, 0.0f, avatar.Value.Width, avatar.Value.Height), new Vector2(0.5f, 0.5f), 100);
    }

    private static async Task<Steamworks.Data.Image?> GetAvatar(SteamId steamId)
    {
        try
        {
            return await SteamFriends.GetMediumAvatarAsync(steamId);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    public static Texture2D Covert(Steamworks.Data.Image image)
    {
        var avatar = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false);

        avatar.filterMode = FilterMode.Trilinear;

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                avatar.SetPixel(x, (int)image.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }

        avatar.Apply();

        return avatar;
    }
}
