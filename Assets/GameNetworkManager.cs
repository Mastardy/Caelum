using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager Singleton { get; private set; } = null;
    public Lobby? CurrentLobby { get; private set; } = null;
    
    private FacepunchTransport transport = null;

    private void Awake()
    {
        if(Singleton == null) Singleton = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    public async void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        
        NetworkManager.Singleton.StartHost();

        CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(4);
    }

    private void Disconnect()
    {
        CurrentLobby?.Leave();
        
        if (NetworkManager.Singleton == null) return;
        
        NetworkManager.Singleton.Shutdown();
    }

    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        transport.targetSteamId = id;
        
        NetworkManager.Singleton.StartClient();
    }
    
    #region NetworkCallbacks

    private void OnServerStarted() => Debug.Log("Server has started!", this);

    private void OnClientConnectedCallback(ulong clientId) => Debug.Log($"Client connected, clientId = {clientId}");

    private void OnClientDisconnectCallback(ulong clientId)
    {
        Debug.Log($"Client disconnected, clientId = {clientId}");
     
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    #endregion
    
    #region SteamCallbacks

    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId) { }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        lobby.SetFriendsOnly();
        lobby.SetData("name", "Wait & Boomer Cave Lobby");
        lobby.SetJoinable(true);
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost) return;
        
        StartClient(lobby.Id);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend) { }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend) { }

    private void OnLobbyInvite(Friend friend, Lobby lobby) => Debug.Log($"You got an invite from {friend.Name}", this);

    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId) => StartClient(lobby.Id);

    #endregion

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
    }

    private void OnApplicationQuit() => Disconnect();
}
