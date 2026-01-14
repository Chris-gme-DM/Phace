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
    private const string FileName = "profile.dat";
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
        _saveFilePath = Path.Combine(Application.persistentDataPath, FileName);
    }
    public void SavePlayerProfile(PlayerProfile profile)
    {
        try
        {
            string json = JsonUtility.ToJson(profile);
            byte[] encrypted = SaveSystem.Encrypt(json);
            File.WriteAllBytes(_saveFilePath, encrypted);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save profile: {ex:Message}");
        }
    }
    public PlayerProfile LoadPlayerProfile()
    {
        if (!File.Exists(_saveFilePath)) return new PlayerProfile();
        try
        {
            byte[] encrypted = File.ReadAllBytes(_saveFilePath);
            string decryptedJson = SaveSystem.Decrypt(encrypted);
            return JsonUtility.FromJson<PlayerProfile>(decryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading profile, returning new: {ex.Message}");
            return new PlayerProfile();
        }
    }
    public static class SaveSystem
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("123456789abcdef");
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("abcdef123456789");
        public static byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;
            using MemoryStream memoryStream = new();
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
            using (StreamWriter writer = new(cryptoStream, Encoding.UTF8))
            {
                writer.Write(plainText);
            }
            return memoryStream.ToArray();
        }

        public static string Decrypt(byte[] cipherData)
        {
            if (cipherData == null || cipherData.Length == 0)
                throw new ArgumentNullException(nameof(cipherData));

            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;
            using MemoryStream memoryStream = new(cipherData);
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
