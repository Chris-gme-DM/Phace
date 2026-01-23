using FishNet;
using FishNet.Object;
using UnityEngine;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
public class GameManager : NetworkBehaviour
{
    #region Settings
    public static GameManager Instance { get; private set; }
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private NetworkObject _enemyPrefab;
    public readonly List<Spacecraft> _playerSpacecrafs = new();
    public readonly List<Spacecraft> _enemySpacecrafts = new();
    public readonly Spacecraft ActiveBoss;
    private LevelData _levelData;
    public readonly SyncVar<int> Level = new();

    private int _nextSpawnIndex = 0;
    #endregion
    #region Initialization
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            if (Instance == null) Instance = this;
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Subscribe to game events here
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChanged);
        GameEvents.OnEntitySpawn.AddListener(HandleEntitySpawn);
        GameEvents.OnPlayerDestroyed.AddListener(HandlePlayerDestroyed);
        GameEvents.OnEnemyDestroyed.AddListener(HandleEnemyDestroyed);

        Level.OnChange += OnLevelChanged;
    }

    public override void OnStopServer()
    {
        GameEvents.OnGameStateChanged.RemoveAllListeners();
        GameEvents.OnEntitySpawn.RemoveAllListeners();
        GameEvents.OnPlayerDestroyed.RemoveAllListeners();
        GameEvents.OnEnemyDestroyed.RemoveAllListeners();

        base.OnStopServer();
    }
    #endregion
    #region Helpers
    public NetworkObject GetPrefabType(AssociationType assoc) => assoc == AssociationType.Player ? _playerPrefab : _enemyPrefab;
    #endregion
    #region EventHandlers
    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.InGame) Level.Value++;
    }
    private void OnLevelChanged(int prev, int next, bool asServer)
    {
        if (!asServer) return;
        HandleLevelChange();
        // This seems convoluted, but in the current setup it is a working bandaid
        GameEvents.OnLevelChanged.Invoke();
    }
    private void HandleLevelChange()
    {
        if (Level.Value <= 1) return;
        // GameSystem knows all the levels, as soon as they exist properly...

        // After levels are properly compiled, enable the next two lines again

        //var levels = GameSystem.Instance.LevelDatas;
        //_levelData = levels[Random.Range(0, levels.Count)];
        // Initialize level with selectedLevel data
        //Debug.Log($"Level {_levelData.LevelID} started with difficulty {_levelData.DifficultyRating}");
        // Additional level start logic here
        // If any player is still dead, respawn them
        foreach(var session in OwnLobbyManager.Instance.ActiveSessions.Values)
        {
            if (session.ControlledSC == null || session.ControlledSC.IsSpawned)
            { 
                SpawnPlayerCraft(session);
            }
        }
        // Read the base on the selectedLevel properties
        // Adjust these settings accordingly
        // Setup enemies, spawn points, etc.    Currently EnemySpawnManager is handling this
    }
    // Make this a Coroutine
    private void HandleEntitySpawn(Spacecraft spacecraft)
    {
        // Initialize spacecraft stats based on its SpacecraftData
        // Read spacecraft stats
        // Identify the spacecraft owner
        // If it is an enemy and needs to be adjusted to the level
        // If it is a player and has adjusted stats, spawn exactly that

    }
    // Make this a Coroutine
    private void HandleEnemyDestroyed()
    {
        // Score the points to the player that destroyed the enemy
        // Check if any active enemies are left
        // Decide if the level is comleted
        foreach (var session in OwnLobbyManager.Instance.ActiveSessions.Values)
        { session.PlayerScore.Value += 100; }
        var esm = EnemySpawnManager.Instance;

    }
    [Server]
    private void HandlePlayerDestroyed()
    {
        // Check if both players are dead
        // if all of them are, initiate GameOver
        // Respawn a player ship with its stats repaired
        foreach(var session in OwnLobbyManager.Instance.ActiveSessions.Values)
        {
            session.PlayerScore.Value -= 500;
            
        }
        if (_playerSpacecrafs.Count > 0) return;
        else
        {
            PostGame(false);
        }
    }

    #endregion
    #region Methods
    [Server]
    private void SpawnPlayerCraft(PlayerSession session)
    {
        Debug.Log($"pleaseSpawnstuff");
        int craftId = session.SpacecraftID.Value;
        var data = GameSystem.Instance.GetSpacecraftDataById(craftId);
        if (data == null) return;
        Transform spawnPoint = _spawnPoints[_nextSpawnIndex];
        _nextSpawnIndex = (_nextSpawnIndex + 1) % _spawnPoints.Length;
        GameObject go = Instantiate(_playerPrefab.gameObject, spawnPoint.position, spawnPoint.rotation);
        InstanceFinder.ServerManager.Spawn(go, session.Owner);
        if (go.TryGetComponent<Spacecraft>(out var spacecraft))
        {
            spacecraft.SpacecraftData = data;
            spacecraft.Initialize(data);
            if (go.TryGetComponent<NetworkObject>(out var netObj)) session.SetControlledSpacecraft(netObj);
            _playerSpacecrafs.Add(spacecraft);
        }
        // maybe deprecated
        GameEvents.OnEntitySpawn.Invoke(spacecraft);
        
    }
    /// <summary>
    /// Currently handled by EnemySpawnManager
    /// </summary>
    private void SpawnEnemy()
    {
        throw new System.NotImplementedException(); 
    }
    private void SpawnBoss()
    {
        throw new System.NotImplementedException(); 
    }
    private void CleanUp()
    {
        // Set the level back to 1, just as a precaution
        Level.Value = 1;
        // Empty the fields
        foreach (var craft in _playerSpacecrafs)
        {
            craft.Despawn();
        }
        _playerSpacecrafs.Clear();
        if(ActiveBoss != null)Despawn();
    }
    [Server]
    public void PostGame(bool victory)
    {
        GameEvents.OnPostGame.Invoke(victory);
        CleanUp();
        OwnLobbyManager.Instance.SetGlobalState(GameState.PostGame);

    }
    #endregion
}