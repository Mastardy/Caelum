using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
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
        foreach (var clit in NetworkManager.Singleton.ConnectedClientsList)
        {
            clit.PlayerObject.GetComponent<Player>().CreateChatEntryClientRpc(message, clientName, client);
        }
    }
}
