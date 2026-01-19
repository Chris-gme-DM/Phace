using FishNet;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

    private void OnGameStateSynced(GameState prev, GameState next, bool asServer)
    {
        GameEvents.ChangeGameState(next);
    }

    [Server]
    public void SetGlobalState(GameState newState)
    {
        _networkedGameState.Value = newState;
        if (IsServerInitialized)
        {
            GameEvents.ChangeGameState(newState);
        }
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
    

    [ServerRpc(RequireOwnership = false)]
    public void RequestJoinLobby(PlayerProfile profile, NetworkConnection caller = null)
    {
        if (caller == null) return;
            
        if (!ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session))
        {
            session = Instantiate(_playerSessionPrefab);
            InstanceFinder.ServerManager.Spawn(session.gameObject, caller);
            ActiveSessions[caller.ClientId] = session;
        }

        // Apply Profile Data
        session.SetFromProfile(profile);

        Debug.Log($"Player {profile.PlayerName} joined");
    }
    [Server]
    private void SpawnPlayer(NetworkConnection conn)
    {
        PlayerSession session = Instantiate(_playerSessionPrefab);
        InstanceFinder.ServerManager.Spawn(session.gameObject, conn);

        ActiveSessions[conn.ClientId] = session;

        LobbyPlayers.Add(conn.ClientId, session.GetSnapshot());

    }
    [ServerRpc(RequireOwnership = false)]
    public void RpcUpdatePlayerData(PlayerProfile profile, NetworkConnection caller = null)
    {
        if (ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session))
        {
            session.SetFromProfile(profile);
            LobbyPlayers[caller.ClientId] = session.GetSnapshot();
        }
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
        SpawnPlayer(conn);
    }
    [Server]
    private void StartGame()
    {
        // Tell that motherfucker to start a countdown and then the fucking game
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