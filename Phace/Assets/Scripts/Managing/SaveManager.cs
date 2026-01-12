using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
/// <summary>
/// Provides functionality for saving and loading the player's profile data to persistent storage. Implements a
/// singleton pattern to ensure a single instance throughout the application's lifetime.
/// </summary>
/// <remarks>The SaveManager persists across scene loads and manages the serialization and deserialization of
/// player profile data using JSON. Access the singleton instance via the Instance property. This class is intended to
/// be attached to a GameObject in the Unity scene.</remarks>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string  _saveFilePath;

    private void Awake()
    {
        if (Instance !=  null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        _saveFilePath = Path.Combine(Application.persistentDataPath, "playerprofile.json");
    }
    public void SavePlayerProfile(PlayerProfile profile)
    {
        string json = JsonUtility.ToJson(profile);
        File.WriteAllText(_saveFilePath, json);
    }
    public PlayerProfile LoadPlayerProfile()
    {
        if (!File.Exists(_saveFilePath)) return new PlayerProfile();
        string json = File.ReadAllText(_saveFilePath);
        return JsonUtility.FromJson<PlayerProfile>(json);
    }
    // Encryption of playerdata
    // Decypher PlayerData
}
