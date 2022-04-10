using System;
using System.Threading.Tasks;
using Steamworks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class Player
{
    private bool inChat;
    
    /// <summary>
    /// Hides the Chat
    /// </summary>
    public void HideChat()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inChat = false;
        takeInput = true;
        EventSystem.current.SetSelectedGameObject(null);
        chatBox.SetActive(false);
    }

    /// <summary>
    /// Opens the Chat
    /// </summary>
    public void OpenChat()
    {
        Cursor.lockState = CursorLockMode.Confined;
        inChat = true;
        takeInput = false;
        chatBox.SetActive(true);
        EventSystem.current.SetSelectedGameObject(chatBox);
    }
    
    /// <summary>
    /// Filters the text from the 
    /// </summary>
    /// <param name="message"></param>
    public void Say(string message)
    {
        // Depois de Descelecionar/Enviar, não queremos que o chat fique à mostra
        HideChat();
        
        // Verificar se a função foi chamada pela tecla Enter
        if (!Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKeyDown(KeyCode.Return)) return;

        // Limpar o Input Field
        chatBox.GetComponent<TMP_InputField>().text = string.Empty;
        
        // Verificar se a mensagem não está vazia
        if (message.Length < 1) return;
        
        // TODO: Regular texto
        
        // Enviar mensagem para o servidor
        SayServerRpc(message, SteamClient.Name, SteamClient.SteamId);
    }
    
    /// <summary>
    /// Sends the ChatEntry to all the connected clients.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="clientName"></param>
    /// <param name="client"></param>
    [ServerRpc(RequireOwnership = false)]
    public void SayServerRpc(string message, string clientName, SteamId client)
    {
        if (!IsServer) return;
        
        foreach (var clit in NetworkManager.Singleton.ConnectedClientsList)
        {
            clit.PlayerObject.GetComponent<Player>().CreateChatEntryClientRpc(message, clientName, client);
        }
    }
    
    /// <summary>
    /// Creates a Chat Entry for the given player
    /// </summary>
    /// <param name="message">Player message</param>
    /// <param name="clientName">Player name</param>
    /// <param name="client">Player SteamID</param>
    [ClientRpc]
    public void CreateChatEntryClientRpc(string message, string clientName, SteamId client)
    {
        var chatEntry = Instantiate(chatEntryPrefab, chatPanel);

        chatEntry.GetComponentInChildren<TextMeshProUGUI>().text = $"<color=#55FF55>{clientName}:</color> {message}";

        var avatar = GetAvatar(client).Result;

        if (avatar == null) return;
        
        chatEntry.GetComponentInChildren<Image>().sprite = Sprite.Create(Convert(avatar.Value), new Rect(0.0f, 0.0f, avatar.Value.Width, avatar.Value.Height), new Vector2(0.5f, 0.5f), 100);
    }

    /// <summary>
    /// Get the Avatar given the SteamID
    /// </summary>
    /// <param name="steamId">SteamID of the Client</param>
    /// <returns></returns>
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

    /// <summary>
    /// Convert the Steam Image to Texture2D
    /// </summary>
    /// <param name="image">Steam given Image</param>
    /// <returns></returns>
    public static Texture2D Convert(Steamworks.Data.Image image)
    {
        // Create the Texture2D
        var avatar = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Trilinear
        };

        // Flip the image
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
