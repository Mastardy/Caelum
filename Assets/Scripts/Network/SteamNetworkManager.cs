using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class SteamNetworkManager : MonoBehaviour
{
    public static SteamNetworkManager Singleton { get; private set; }
    public Lobby? CurrentLobby { get; set; }
    
    public FacepunchTransport transport;
    public UnityTransport unityTransport;
    
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
        unityTransport = GetComponent<UnityTransport>();

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    public void StartSingleplayer()
    {
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = unityTransport;
        NetworkManager.Singleton.StartHost();
    }
    
    public async void StartHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        
        NetworkManager.Singleton.StartHost();

        CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        transport.targetSteamId = id;
        
        Debug.Log($"Joining room hosted by {transport.targetSteamId}", this);

        if (NetworkManager.Singleton.StartClient()) Debug.Log("Client has joined!", this);
    }

    public void Disconnect()
    {
        CurrentLobby?.Leave();
        
        if (!NetworkManager.Singleton) return;
        
        NetworkManager.Singleton.Shutdown();
    }

    #region NetworkCallbacks

    private void OnServerStarted() { }

    private void OnClientConnectedCallback(ulong clientId) { }

    private void OnClientDisconnectCallback(ulong clientId) 
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    #endregion
    
    #region SteamCallbacks

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError($"Lobby couldn't be created!, {result}", this);
            return;
        }
        
        lobby.SetPublic();
        lobby.SetData("name", "Cool Lobby");
        lobby.SetJoinable(true);

        Debug.Log("Lobby has been created!");
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        Debug.Log($"You have entered in lobby, clientId = {NetworkManager.Singleton.LocalClientId}", this);
        
        if (NetworkManager.Singleton.IsHost) return;
        
        StartClient(lobby.Owner.Id);
    }

    private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log("On Lobby Member Joined");
    }

    private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Debug.Log("On Lobby Member Leave");
    }

    private void OnLobbyInvite(Friend friend, Lobby lobby) => Debug.Log($"You got an invite from {friend.Name}");

    private void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log("On Game Lobby Join Requested");

        bool isSame = lobby.Owner.Id.Equals(steamId);
        
        Debug.Log($"Owner: {lobby.Owner}");
        Debug.Log($"Id: {steamId}");
        Debug.Log($"IsSame: {isSame}");
        
        StartClient(steamId);
    }

    #endregion

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;

        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }

    private void OnApplicationQuit() => Disconnect();
}
