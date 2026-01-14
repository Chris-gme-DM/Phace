using UnityEngine;
using UnityEngine.Events;
using System;
/// <summary>
/// This script holds game-related data. Levels, settings, global stats, and structs used across multiple systems.
/// </summary>
/// 
public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    [SerializeField] private GameData _activeGameData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    public void CreateNewGame()
    {
        _activeGameData = new GameData();
    }
    public void SetGameState(GameState newState)
    {
        _activeGameData.CurrentState = newState;
        GameEvents.ChangeGameState(newState);
    }
}
#region Scriptable Objects
[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/Game Data")]
public class GameData : ScriptableObject
{
    private int _Level;
    public int Level => _Level;

    public GameState CurrentState;

}

[CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _levelID;
    [SerializeField] private int _difficultyRating;
    [SerializeField] private EnemyData[] _enemies;
    [SerializeField] private Transform[] _spawnPoints;

    public int LevelID => _levelID;
    public int DifficultyRating => _difficultyRating;
    public EnemyData[] Enemies => _enemies;
    public Transform[] SpawnPoints => _spawnPoints;
}
#endregion
#region Structs
[Serializable]
public struct PlayerLobbyData
{
    public int PlayerID;
    public string PlayerName;
    public int SelectedSpacecraftID;
    public bool IsReady;
}

#endregion
#region Enums
public enum AssociationType
{
    Player,
    Enemy,
    Neutral
}
public enum GameState
{
    MainMenu,
    Lobby,
    InGame,
    PostGame,
}

#endregion
#region Events
[Serializable]
public static class GameEvents
{
    public static UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();
    public static UnityEvent<PlayerLobbyData> OnPlayerStatusChanged = new UnityEvent<PlayerLobbyData>();
    public static UnityEvent<Spacecraft> OnEntitySpawn = new UnityEvent<Spacecraft>();
    public static UnityEvent OnPlayerDestroyed = new UnityEvent();
    public static UnityEvent OnEnemyDestroyed = new UnityEvent();
    public static UnityEvent OnLevelChanged = new UnityEvent();

    public static void ChangeGameState(GameState newState)
    {
        OnGameStateChanged.Invoke(newState);
    }
}
#endregion