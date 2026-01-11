using UnityEngine;
/// <summary>
/// This script holds game-related data. Levels, settings, global stats, and structs used across multiple systems.
/// </summary>
/// 
#region Scriptable Objects
[CreateAssetMenu(fileName = "GameData", menuName = "Game Data/Game Data")]
public class GameData : ScriptableObject
{
    [SerializeField] private int _Level;
    [SerializeField] private float _defaultVolume;
    [SerializeField] private float _defaultBrightness;
    public int Level => _Level;
    public float DefaultVolume => _defaultVolume;
    public float DefaultBrightness => _defaultBrightness;
    public GameState CurrentState;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _levelID;
    [SerializeField] private int _difficultyRating;
    [SerializeField] private EnemyData[] _enemies;
    [SerializeField] private Transform[] _spawnPoints;
}
#endregion
#region Structs

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