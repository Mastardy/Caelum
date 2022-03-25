using Steamworks;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    [ServerRpc(RequireOwnership = false)]
    public void SayServerRpc(string message, string clientName, SteamId client)
    {
        Debug.Log("Say command");
        
        // Ordenar aos clients que criem um Chat Entry
        foreach (var clit in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log("player detected");
            Debug.Log(clit.PlayerObject.GetComponent<Player>().name);
            clit.PlayerObject.GetComponent<Player>().CreateChatEntryClientRpc(message, clientName, client);
        }
    }
}
