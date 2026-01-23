using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [Header("Information")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;
    [Header("Configuration")]
    [SerializeField] private GameObject _masterVolumeSlider;
    [SerializeField] private GameObject _musicVolumeSlider;
    [SerializeField] private GameObject _sfxVolumeSlider;
    public float MasterVolume => _masterVolume;
    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;

    /// <summary>
    /// Currently the SoundManager is not compiled yet, but will be added later.
    /// </summary>
    /// <param name="amount"></param>
    public void OnMasterVolumeEdit(float amount)
    { 
        _masterVolume = amount;
        // Save this to the PlayerProfile

    }
    public void OnMusicVolumeEdit(float amount)
    {
        _musicVolume = amount;
        // Save this to the PlayerProfile
    }
    public void OnSfxVolumeEdit(float amount)
    {
        _sfxVolume = amount;
        // Save this to the PlayerProfile

    }
    public void OnClickSelfDestruct()
    {
        // Deal over 9000 dmg to this players object
    }
    public void OnClickMainMenu()
    {
        OwnLobbyManager.Instance.SetGlobalState(GameState.MainMenu);
        SaveManager.Instance.SavePlayerProfile(GameSystem.Instance.ActiveProfile);
    }
    public void OnClickQuit()
    {
        SaveManager.Instance.SavePlayerProfile(GameSystem.Instance.ActiveProfile);
        Application.Quit();
    }
}
