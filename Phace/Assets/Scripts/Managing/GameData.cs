using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
/// <summary>
/// This script holds game-related data. Levels, settings, global stats, and structs used across multiple systems.
/// </summary>
public class GameData : ScriptableObject
{
    [SerializeField] private int _Level;
    [SerializeField] private float _defaultVolume;
    [SerializeField] private float _defaultBrightness;
    public int Level => _Level;
    public float DefaultVolume => _defaultVolume;
    public float DefaultBrightness => _defaultBrightness;
}
public enum AssociationType
{
    Player,
    Enemy,
    Neutral
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _levelID;
    [SerializeField] private int _difficultyRating;
    [SerializeField] private EnemyData[] _enemies;
}