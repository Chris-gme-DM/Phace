using FishNet;
using FishNet.Object;
using UnityEngine;
using System.Collections;
public class GameManager : NetworkBehaviour
{
    #region Settings
    public static GameManager Instance { get; private set; }

    [SerializeField] private SpacecraftData[] _spaceCraftDatas;
    [SerializeField] private LevelData[] _levelDatas;
    #endregion
    #region Initialization
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        Instance = this;
        SpawnPlayers();
        // Subscribe to game events here
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChanged);
        GameEvents.OnEntitySpawn.AddListener(HandleEntitySpawn);
        GameEvents.OnPlayerDestroyed.AddListener(HandlePlayerDestroyed);
        GameEvents.OnEnemyDestroyed.AddListener(HandleEnemyDestroyed);
        GameEvents.OnLevelChanged.AddListener(HandleLevelChanged);
    }

    public override void OnStopNetwork()
    {
        GameEvents.OnGameStateChanged.RemoveAllListeners();
        GameEvents.OnEntitySpawn.RemoveAllListeners();
        GameEvents.OnPlayerDestroyed.RemoveAllListeners();
        GameEvents.OnEnemyDestroyed.RemoveAllListeners();
        GameEvents.OnLevelChanged.RemoveAllListeners();

        base.OnStopNetwork();
        Instance = null;
    }
    #endregion
    #region EventHandlers
    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.InGame) HandleLevelChanged();
        if (newState != GameState.InGame) CleanUp();
    }
    private void HandleLevelChanged()   // Effectively if current game level is 1 is starting a new game
    {
        LevelData selectedLevel = _levelDatas[Random.Range(0, _levelDatas.Length)];
        // Initialize level with selectedLevel data
        Debug.Log($"Level {selectedLevel.LevelID} started with difficulty {selectedLevel.DifficultyRating}");
        // Additional level start logic here
        // Read the base on the selectedLevel properties
        // Adjust these settings accordingly
        // Setup enemies, spawn points, etc.
    }
    // Make this a Coroutine
    private void HandleEntitySpawn(Spacecraft spacecraft)
    {
        // Initialize spacecraft stats based on its SpacecraftData
        // Read spacecraft stats
        // If it is an enemy and needs to be adjusted to the level
        // If it is a player and has adjusted stats, spawn exactly that
        throw new System.NotImplementedException();

    }
    // Make this a Coroutine
    private void HandleEnemyDestroyed()
    {
        // Score the points to the player that destroyed the enemy
        // Check if any active enemies are left
        // Decide if the level is comleted
        throw new System.NotImplementedException();
    }

    private void HandlePlayerDestroyed()
    {
        // Check if both players are dead
        // if all of them are, initiate GameOver
        // Respawn a player ship with its stats repaired
        throw new System.NotImplementedException();
    }

    #endregion
    #region Methods
    private void SpawnPlayers()
    {
        foreach (var session in LobbyManager.Instance.ActiveSessions.Values)
        {
            SpacecraftData data = System.Array.Find(_spaceCraftDatas, sc => sc.SpacecraftID == session.SpacecraftID.Value);
            if (data != null)
            {
                GameObject obj = Instantiate(data.SpacecraftPrefab);
            }
        }
    }
    private void CleanUp()
    {
        // Set the level back to 1, just as a precaution
        // Empty the field
    }
    // Method to check if any enemy is alive
    #endregion
}