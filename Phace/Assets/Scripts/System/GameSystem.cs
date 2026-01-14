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

    public GameState CurrentState;

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
    public void ApplyState(GameState newState)
    {
        CurrentState = newState;
    }
}
#region Scriptable Objects

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
public struct PlayerInfo
{
    public int PlayerID;
    public string PlayerName;
    public int SpacecraftID;
    public bool IsReady;
}
[Serializable]
public struct PlayerSummary
{
    public string PlayerName;
    public int SpacecraftID;
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
    public static UnityEvent<GameState> OnGameStateChanged = new();
    public static UnityEvent<PlayerInfo> OnPlayerStatusChanged = new();
    public static UnityEvent<Spacecraft> OnEntitySpawn = new();
    public static UnityEvent OnPlayerDestroyed = new();
    public static UnityEvent OnEnemyDestroyed = new();
    public static UnityEvent OnLevelChanged = new();

    public static void ChangeGameState(GameState newState)
    {
        try
        {
            OnGameStateChanged?.Invoke(newState);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception while invoking OnGameStateChanged for state{newState}: {ex}");
        }
        Debug.Log($"GameState changed to {newState}");
        Debug.Log($", GameSystem: {GameSystem.Instance.CurrentState}");
        GameSystem.Instance.ApplyState(newState);
    }
}
#endregion