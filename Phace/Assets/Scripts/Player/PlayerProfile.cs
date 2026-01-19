using System;
/// <summary>
/// This script holds player-related data. ID, stats, high scores, etc.
/// </summary>
/// 
[Serializable]
public class PlayerProfile
{
    // Player Info
    public int PlayerID;
    public string PlayerName;
    public int HighScore;
    public int SelectedSpacecraftID;

    // Settings
    public float PreferredMasterVolume = 1f;
    public float PreferredMusicVolume = 1f;
    public float PreferredSFXVolume = 1f;

}