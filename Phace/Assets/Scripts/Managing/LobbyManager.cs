using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;
using System.Collections.Generic;
using FishNet;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;

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

        SceneLoadData sld = new(_lobbySceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
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
        if (ActiveSessions.Count == 0)
            return;
        foreach (var session in ActiveSessions.Values)
        {
            if (!session.IsReady.Value) return;
        }
        StartGame();
    }

    private void StartGame()
    {
        SceneLoadData sld = new SceneLoadData(_gameSceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

}
