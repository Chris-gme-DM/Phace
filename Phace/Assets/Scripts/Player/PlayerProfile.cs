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
    public int PreferredMasterVolume;
    public int PreferredMusicVolume;
    public int PreferredSFXVolume;

}