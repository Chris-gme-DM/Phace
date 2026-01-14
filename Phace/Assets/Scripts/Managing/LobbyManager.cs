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
    public readonly SyncVar<GameState> CurrentState = new();

    public readonly Dictionary<int, PlayerSession> ActiveSessions = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Instance = this;
        // Subscribe to additional server events here
        GameEvents.OnPlayerStatusChanged.AddListener(HandleStatusChange);

        CurrentState.OnChange += OnNetworkStateChanged_Server;

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        CurrentState.OnChange += OnNetworkChanged_Client;

        GameEvents.ChangeGameState(CurrentState.Value);
    }

    private void OnNetworkStateChanged_Server(GameState prev, GameState next, bool asServer)
    {
        if (asServer && IsHostStarted)
        {
            GameEvents.ChangeGameState(next);

        }
    }

    private void OnNetworkChanged_Client(GameState prev, GameState next, bool asServer)
    {
        GameEvents.ChangeGameState(next);
        if (next == GameState.Lobby) EnterLobby();
        if (next == GameState.InGame) StartGame();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestChangeState(GameState request)
    {
        CurrentState.Value = request;
    }
    private void HandleStatusChange(PlayerInfo arg0)
    {
        throw new NotImplementedException();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        foreach (var session in ActiveSessions.Values)
        {
            if (session != null)
            {
                InstanceFinder.ServerManager.Despawn(session);
            }
            ActiveSessions.Clear();

            GameSystem.Instance.ApplyState(GameState.MainMenu);
        }
    }

    [ServerRpc]
    private void EnterLobby()
    {
        SceneLoadData sld = new(_lobbySceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        UIManager.Instance.ShowLoadingScreen();
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
        
        // StartGame();
    }
    /// <summary>
    /// Host is able to start the game after all players are ready
    /// </summary>
    [Server]
    public void StartGame()
    {
        SceneLoadData sld = new(_gameSceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
        GameEvents.ChangeGameState(GameState.InGame);
        UIManager.Instance.ShowLoadingScreen();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestJoinLobby(PlayerSummary summ, NetworkConnection conn = null)
    {
        NetworkConnection caller = conn ?? base.Owner;
        if (caller == null) return;

        if (summ.SpacecraftID < 0)
            summ.SpacecraftID = 0;
        if (!ActiveSessions.TryGetValue(caller.ClientId, out PlayerSession session) || session == null)
        {
            session = Instantiate(_playerSessionPrefab);
            InstanceFinder.ServerManager.Spawn(session.gameObject, caller);
            ActiveSessions[caller.ClientId] = session;
        }

        var profile = new PlayerProfile { PlayerID = caller.ClientId, PlayerName = summ.PlayerName, SelectedSpacecraftID = summ.SpacecraftID };
        session.SetFromProfile(profile);

        GameEvents.OnPlayerStatusChanged.Invoke(new PlayerInfo
        {
            PlayerID = conn.ClientId,
            PlayerName = summ.PlayerName,
            SpacecraftID = summ.SpacecraftID,
            IsReady = false,
        });
    }
}
