using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameKit.Dependencies.Utilities.Types;

public class OwnLobbyManager : SingletonNetworkBehaviour<OwnLobbyManager>
{
    #region Configuration
    [Header("Scene Settings")]
    [SerializeField, Min(1)] private int _maxPooledScenes = 1;
    [GameKit.Dependencies.Utilities.Types.Scene, SerializeField] private string _lobbyScene;
    [Scene, SerializeField] private string _gameScene;

    [Header("Lobby Settings")]
    [Min(1)] public int MaxLobbyClients = 4;
    [Min(1)] public int MinLobbyClients = 1;
    [Header("Prefabs")]
    [SerializeField] private PlayerSession _playerSessionPrefab;
    #endregion

    #region State
    // Tracks all active lobbies (Rooms)
    private readonly List<Lobby> _lobbies = new();

    // Tracks active PlayerSessions by ClientId
    public readonly Dictionary<int, PlayerSession> ActiveSessions = new();

    // Scene Pooling Lists
    private readonly List<UnityEngine.SceneManagement.Scene> _pooledLobbyScenes = new();
    private readonly List<UnityEngine.SceneManagement.Scene> _pooledGameScenes = new();
    #endregion

    protected override void Awake()
    {
        base.Awake();
        // Basic validation to ensure you didn't forget to assign scenes
        if (string.IsNullOrEmpty(_lobbyScene) || string.IsNullOrEmpty(_gameScene))
        {
            Debug.LogError("OwnLobbyManager: Scene names are missing in Inspector!");
            enabled = false;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Start pooling scenes so they are ready when players join
        LobbyScenePooling();
        GameScenePooling();

        // Listen for disconnects to clean up players
        InstanceFinder.ServerManager.OnRemoteConnectionState += RemoteConnectionStateChanged;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        InstanceFinder.ServerManager.OnRemoteConnectionState -= RemoteConnectionStateChanged;
        _pooledLobbyScenes.Clear();
        _pooledGameScenes.Clear();
        _lobbies.Clear();
        ActiveSessions.Clear();
    }

    #region Public API (Your Game Logic)

    [ServerRpc(RequireOwnership = false)]
    public void RequestJoinLobby(PlayerProfile profile, NetworkConnection caller = null)
    {
        Lobby lobby = GetAvailableLobby();
        lobby.ClientJoin(caller);

        if (!ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session))
        {
            session = Instantiate(_playerSessionPrefab);
            InstanceFinder.ServerManager.Spawn(session.gameObject, caller);
            ActiveSessions[caller.ClientId] = session;
        }

        // Apply Profile Data
        session.SetFromProfile(profile);
        LoadLobbySceneForClient(lobby, caller);

        Debug.Log($"Player {profile.PlayerName} joined Lobby {lobby.Id}");
    }

    [Server]
    public void CheckAllPlayersReady(NetworkConnection sender)
    {
        Lobby lobby = FindLobbyOfClient(sender);
        if (lobby == null) return;

        if (lobby.Clients.Length < MinLobbyClients) return;

        // Check if everyone in THIS lobby is ready
        foreach (var client in lobby.Clients)
        {
            if (ActiveSessions.TryGetValue(client.ClientId, out PlayerSession session))
            {
                if (!session.IsReady.Value) return; // Someone isn't ready
            }
        }
        // If everyone is ready show the start button 
        StartGame(lobby);
    }

    [Server]
    private void StartGame(Lobby lobby)
    {
        SwitchToGameScene(lobby);
    }

    #endregion

    #region Functions from LobbyManager

    [Server]
    private Lobby GetAvailableLobby()
    {
        // Try to find an open lobby
        Lobby lobby = _lobbies.Find(l => l.CanJoin);
        // If none exist, create a new one
        if (lobby == null)
        {
            lobby = CreateNewLobby();
        }
        return lobby;
    }

    [Server]
    public Lobby FindLobbyOfClient(NetworkConnection client) => _lobbies.FirstOrDefault(x => x.HasClient(client));

    [Server]
    private void RemoteConnectionStateChanged(NetworkConnection client, FishNet.Transporting.RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != FishNet.Transporting.RemoteConnectionState.Stopped) return;

        // Clean up Session
        if (ActiveSessions.ContainsKey(client.ClientId))
            ActiveSessions.Remove(client.ClientId);

        // Clean up Lobby
        Lobby lobby = FindLobbyOfClient(client);
        if (lobby != null)
        {
            lobby.ClientLeft(client);
            // Optional: If lobby is empty, clean it up?
            if (lobby.Clients.Length == 0) CleanupLobby(lobby);
        }
    }
    [Server]
    private Lobby CreateNewLobby()
    {
        Lobby lobby = null;
        if (_pooledLobbyScenes.Count > 0)
        {
            lobby = new Lobby(_pooledLobbyScenes[0]);
            _lobbies.Add(lobby);
            _pooledLobbyScenes.RemoveAt(0);
        }
        else
        {
            Debug.LogWarning("No pooled lobby scenes available! Waiting for pool...");
        }

        // Replenish the pool
        LobbyScenePooling();
        return lobby;
    }

    [Server]
    private void LoadLobbySceneForClient(Lobby lobby, NetworkConnection client)
    {
        SceneLoadData sld = new SceneLoadData(lobby.Scene);
        sld.Options.AllowStacking = true;
        sld.ReplaceScenes = ReplaceOption.All; // Unload Main Menu, load Lobby
        sld.MovedNetworkObjects = new NetworkObject[] { client.Objects.ElementAt(0) }; // Move the Player (if it exists)
        InstanceFinder.SceneManager.LoadConnectionScenes(client, sld);
    }

    [Server]
    public void SwitchToGameScene(Lobby lobby)
    {
        lobby.StartLobby(); // Locks the lobby
        UnityEngine.SceneManagement.Scene oldLobbyScene = lobby.Scene;

        if (_pooledGameScenes.Count > 0)
        {
            // Assign new Game Scene
            lobby.Scene = _pooledGameScenes[0];
            _pooledGameScenes.RemoveAt(0);

            // Move everyone to the new Game Scene
            SceneLoadData sld = new SceneLoadData(lobby.Scene);
            sld.Options.AllowStacking = true;
            sld.ReplaceScenes = ReplaceOption.All;

            // Grab all player objects to move
            List<NetworkObject> objectsToMove = new List<NetworkObject>();
            foreach (var client in lobby.Clients)
            {
                // Add the connection's main object
                if (client.FirstObject != null) objectsToMove.Add(client.FirstObject);
                // Add their PlayerSession object too!
                if (ActiveSessions.TryGetValue(client.ClientId, out PlayerSession session))
                    objectsToMove.Add(session.NetworkObject);
            }
            sld.MovedNetworkObjects = objectsToMove.ToArray();

            InstanceFinder.SceneManager.LoadConnectionScenes(lobby.Clients, sld);
        }

        // Replenish pools and cleanup old scene
        GameScenePooling();
        StartCoroutine(CoroUnloadScene(oldLobbyScene));
    }

    [Server]
    public void CleanupLobby(Lobby lobby)
    {
        UnityEngine.SceneManagement.Scene lobbyScene = lobby.Scene;
        _lobbies.Remove(lobby);
        StartCoroutine(CoroUnloadScene(lobbyScene));
    }

    private IEnumerator CoroUnloadScene(UnityEngine.SceneManagement.Scene scene)
    {
        // Wait until clients are gone (basic check)
        yield return new WaitForSeconds(1f);
        InstanceFinder.SceneManager.UnloadConnectionScenes(new SceneUnloadData(scene));
    }
    #endregion
    #region Pooling Logic
    [Server]
    private void LobbyScenePooling()
    {
        if (_pooledLobbyScenes.Count >= _maxPooledScenes) return;
        InstanceFinder.SceneManager.OnLoadEnd += LobbyScenePooling_OnLoadEnd;
        LoadSceneToPool(_lobbyScene);
    }

    [Server]
    private void GameScenePooling()
    {
        if (_pooledGameScenes.Count >= _maxPooledScenes) return;
        InstanceFinder.SceneManager.OnLoadEnd += GameScenePooling_OnLoadEnd;
        LoadSceneToPool(_gameScene);
    }

    private void LoadSceneToPool(string sceneName)
    {
        SceneLoadData sld = new SceneLoadData(sceneName);
        sld.Options.AllowStacking = true;
        sld.Options.AutomaticallyUnload = false; 
        InstanceFinder.SceneManager.LoadConnectionScenes(sld);
    }

    private void LobbyScenePooling_OnLoadEnd(SceneLoadEndEventArgs args)
    {
        if (HandlePoolLoad(args, _lobbyScene, _pooledLobbyScenes))
        {
            if (_pooledLobbyScenes.Count >= _maxPooledScenes)
                InstanceFinder.SceneManager.OnLoadEnd -= LobbyScenePooling_OnLoadEnd;
        }
    }

    private void GameScenePooling_OnLoadEnd(SceneLoadEndEventArgs args)
    {
        if (HandlePoolLoad(args, _gameScene, _pooledGameScenes))
        {
            if (_pooledGameScenes.Count >= _maxPooledScenes)
                InstanceFinder.SceneManager.OnLoadEnd -= GameScenePooling_OnLoadEnd;
        }
    }

    private bool HandlePoolLoad(SceneLoadEndEventArgs args, string targetScene, List<UnityEngine.SceneManagement.Scene> pool)
    {
        if (args.LoadedScenes.Length != 1) return false;
        UnityEngine.SceneManagement.Scene loaded = args.LoadedScenes[0];
        if (loaded.path != targetScene && loaded.name != targetScene) return false;

        if (!pool.Contains(loaded))
        {
            pool.Add(loaded);
            // Hide the scene physically so it doesn't clutter the view? 
            // Usually you might move root objects far away, but for now we just hold it.
            return true;
        }
        return false;
    }
    #endregion
}