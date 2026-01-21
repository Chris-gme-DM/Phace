using FishNet;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class OwnLobbyManager : SingletonNetworkBehaviour<OwnLobbyManager>
{
    #region Configuration
    [Header("Lobby Settings")]
    [Min(1)] public int MaxLobbyClients = 4;
    [Min(1)] public int MinLobbyClients = 1;
    [Header("Prefabs")]
    [SerializeField] private PlayerSession _playerSessionPrefab;

    public readonly Dictionary<int, PlayerSession> ActiveSessions = new();
    public readonly SyncDictionary<int, PlayerSessionData> LobbyPlayers = new();

    #endregion
    #region Initialization
    public override void OnStartServer()
    {
        base.OnStartServer();
        _networkedGameState.OnChange += OnGameStateSynced;
        InstanceFinder.ServerManager.OnRemoteConnectionState += RemoteConnectionStateChanged;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedScenes;

        SetGlobalState(GameState.Lobby);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        _networkedGameState.OnChange += OnGameStateSynced;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        _networkedGameState.OnChange -= OnGameStateSynced;
        InstanceFinder.ServerManager.OnRemoteConnectionState -= RemoteConnectionStateChanged;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedScenes;

        ActiveSessions.Clear();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        _networkedGameState.OnChange -= OnGameStateSynced;
    }
    #endregion
    #region State
    [AllowMutableSyncType] private readonly SyncVar<GameState> _networkedGameState = new();

    [Server]
    public void SetGlobalState(GameState newState)
    {
        Debug.Log($"Server: Requesting state change to {newState}");
        _networkedGameState.Value = newState;
        GameEvents.ChangeGameState(newState);
        Debug.Log($"THE GAME STATE IS: {newState}, MF");
    }
    private void OnGameStateSynced(GameState prev, GameState next, bool asServer)
    {
        Debug.Log($"[SyncVar] GameState changed from {prev} to {next}");
        HandleStateChange(next);
    }
    private void HandleStateChange(GameState newState)
    {
        GameEvents.ChangeGameState(newState);
        Debug.Log($"GameState: {newState}");
    }
    #endregion
    #region Handlers
    [Server]
    private void SpawnPlayerSession(NetworkConnection conn)
    {
        if (!ActiveSessions.TryGetValue(conn.ClientId, out PlayerSession session))
        {
            session = Instantiate(_playerSessionPrefab);
            InstanceFinder.ServerManager.Spawn(session.gameObject, conn);
            ActiveSessions[conn.ClientId] = session;
        }
        LobbyPlayers.Add(conn.ClientId, session.GetSnapshot());

    }

    [ServerRpc(RequireOwnership = false)]
    public void RpcRequestProfileUpdate(PlayerSessionData data, NetworkConnection caller = null)
    {
        if (caller == null || !ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session))
            return;

        UpdatePlayerSessionInternal(session, data);
    }

    [Server]
    private void UpdatePlayerSessionInternal(PlayerSession session, PlayerSessionData data)
    {
        session.PlayerName.Value = data.PlayerName;
        session.SpacecraftID.Value = data.SpacecraftID;
        session.IsReady.Value = data.IsReady;

        LobbyPlayers[session.Owner.ClientId] = session.GetSnapshot();

        OnPlayerReadyStatusChanged();
    }

    [Server]
    public void OnPlayerReadyStatusChanged()
    {
        if (ActiveSessions.Count < MinLobbyClients) return;
        bool allReady = true;
        foreach (var session in ActiveSessions.Values)
        {
            if (!session.IsReady.Value)
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            if(ActiveSessions.Count >= MinLobbyClients)
            // Enable the start game button in the Host UI
            UIManager.Instance.StartButton.SetActive(true);
        }
    }
    [Server]
    private void OnClientLoadedScenes(NetworkConnection conn, bool asServer)
    {
        if (!ActiveSessions.ContainsKey(conn.ClientId))
        SpawnPlayerSession(conn);
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void RpcRequestStartGame()
    {
        Instance.StartGame();
    }
    [Server]
    private void StartGame()
    {
        Debug.Log("UI: Host Button Clicked");
        // Tell that motherfucker to start a countdown and then the fucking game
        Instance.SetGlobalState(GameState.InGame);
        UIManager.Instance.StartButton.SetActive(false);
    }

    [Server]
    private void RemoteConnectionStateChanged(NetworkConnection client, RemoteConnectionStateArgs args)
    {
        //if (args.ConnectionState == RemoteConnectionState.Started)
        //{
        //    // Spawning Logic of PLayerSessions and their fucking data
        //    SpawnPlayer(client);
        //}
        if (args.ConnectionState != RemoteConnectionState.Stopped)
        {
            if (ActiveSessions.TryGetValue(client.ClientId, out PlayerSession session))
            {
                session.NetworkObject.Despawn();
                ActiveSessions.Remove(client.ClientId);
            }
            LobbyPlayers.Remove(client.ClientId);
        }
    }
    #endregion
}
