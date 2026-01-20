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

    #region State
    [AllowMutableSyncType] private readonly SyncVar<GameState> _networkedGameState = new();

    [Server]
    public void SetGlobalState(GameState newState)
    {
        _networkedGameState.Value = newState;
        if (IsServerInitialized)
        {
            GameEvents.ChangeGameState(newState);
            Debug.Log($"THE GAME STATE IS: {newState}, MF");
        }
    }
    private void OnGameStateSynced(GameState prev, GameState next, bool asServer)
    {
        if (IsClientInitialized)
        {
            HandleStateChange(next);
        }
    }
    private void HandleStateChange(GameState newState)
    {
        GameEvents.ChangeGameState(newState);
    }
    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();
        _networkedGameState.OnChange += OnGameStateSynced;
        InstanceFinder.ServerManager.OnRemoteConnectionState += RemoteConnectionStateChanged;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += OnClientLoadedScenes;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        _networkedGameState.OnChange -= OnGameStateSynced;
        InstanceFinder.ServerManager.OnRemoteConnectionState -= RemoteConnectionStateChanged;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedScenes;

        ActiveSessions.Clear();
    }

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
        // Validate the connection
        if (caller == null || !ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session))
            return;

        // Use the internal server method to apply the data
        UpdatePlayerSessionInternal(session, data);
    }

    [Server]
    private void UpdatePlayerSessionInternal(PlayerSession session, PlayerSessionData data)
    {
        session.PlayerName.Value = data.PlayerName;
        session.SpacecraftID.Value = data.SpacecraftID;
        session.IsReady.Value = data.IsReady;

        LobbyPlayers[session.Owner.ClientId] = session.GetSnapshot();

        Debug.Log($"[Server] Session synced for: {data.PlayerName}");
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
            // Enable the start game button in the Host UI
        }
    }
    [Server]
    private void OnClientLoadedScenes(NetworkConnection conn, bool asServer)
    {
        if (!ActiveSessions.ContainsKey(conn.ClientId))
        SpawnPlayerSession(conn);
        
    }
    [Server]
    private void StartGame()
    {
        // Tell that motherfucker to start a countdown and then the fucking game
        SetGlobalState(GameState.InGame);
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
}