using UnityEngine;
using System;
/// <summary>
/// This script holds player-related data. ID, stats, high scores, etc.
/// </summary>
public class PlayerData : ScriptableObject
{
    private int _playerID;
    private string _playerName;
    private string _score;
    private int _highScore;
    private Spacecraft _selectedSpacecraft;

    public int PlayerID => _playerID;
    public string PlayerName => _playerName;
    public string Score => _score;
    public int HighScore => _highScore;
    public Spacecraft SelectedSpacecraft => _selectedSpacecraft;
}