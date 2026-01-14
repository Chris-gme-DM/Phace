using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;
using System;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private PlayerSession _playerSessionPrefab;
    [SerializeField] private string _gameSceneName;
    [SerializeField] private string _lobbySceneName;

    public readonly Dictionary<int, PlayerSession> ActiveSessions = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Instance = this;
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        // Subscribe to additional server events here
        GameEvents.OnPlayerStatusChanged.AddListener(HandleStatusChange);
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Lobby) EnterLobby();
        if (newState == GameState.InGame) StartGame();
    }

    private void HandleStatusChange(PlayerLobbyData arg0)
    {
        throw new NotImplementedException();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
    }
    private void EnterLobby()
    {
        SceneLoadData sld = new(_lobbySceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        UIManager.Instance.ShowLoadingScreen();
    }
    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if(args.ConnectionState == RemoteConnectionState.Started)
        {
            SpawnPlayerSession(conn);
        }
        else if(args.ConnectionState == RemoteConnectionState.Stopped)
        {
            ActiveSessions.Remove(conn.ClientId);
        }
    }
    private void SpawnPlayerSession(NetworkConnection conn)
    {
        PlayerSession session = Instantiate(_playerSessionPrefab);
        InstanceFinder.ServerManager.Spawn(session.gameObject, conn);
        ActiveSessions[conn.ClientId] = session;
    }

    [Server]
    public void CheckAllPlayersReady()
    {
        if (ActiveSessions.Count <= 2)  // Minimum players to start
            return;
        foreach (var session in ActiveSessions.Values)
        {
            if (!session.IsReady.Value) return;
        }
        // Move this to the button handler later
        // StartGame();
    }

    [Server]
    public void StartGame()
    {
        SceneLoadData sld = new SceneLoadData(_gameSceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        UIManager.Instance.ShowLoadingScreen();
    }

}
