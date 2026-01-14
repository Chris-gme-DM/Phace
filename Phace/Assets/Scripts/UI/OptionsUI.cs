using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

    public float MasterVolume => _masterVolume;
    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;

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

}
