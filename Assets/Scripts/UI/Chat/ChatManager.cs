using System;
using Steamworks;
using Steamworks.Data;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    [SerializeField] private Transform chatPanel;
    [SerializeField] private GameObject chatEntryPrefab;
    
    public void Say(string message)
    {
        // Verificar se a função foi chamada pela tecla Enter
        if (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return)) return;

        // Verificar se a mensagem não está vazia
        if (message.Length < 1) return;
        
        // TODO: Regular texto
        
        // Enviar mensagem para o servidor
        SayServerRpc(message, SteamClient.Name, SteamClient.SteamId);
    }
    
    [ServerRpc]
    public void SayServerRpc(string message, string clientName, SteamId client)
    {
        // Ordenar aos clients que criem um Chat Entry
        CreateChatEntryClientRpc(message, clientName, client);
    }

    [ClientRpc]
    public void CreateChatEntryClientRpc(string message, string clientName, SteamId client)
    {
        var chatEntry = Instantiate(chatEntryPrefab, chatPanel);

        chatEntry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=#55FF55>{clientName}:</color> {message}";

        var avatar = GetAvatar(client).Result;

        if (avatar == null) return;
        
        chatEntry.GetComponentInChildren<UnityEngine.UI.Image>().sprite = Sprite.Create(Covert(avatar.Value), new Rect(0.0f, 0.0f, avatar.Value.Width, avatar.Value.Height), new Vector2(0.5f, 0.5f), 100);
    }

    private static async Task<Image?> GetAvatar(SteamId steamId)
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

    public static Texture2D Covert(Image image)
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
