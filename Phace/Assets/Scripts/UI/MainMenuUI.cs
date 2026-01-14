using FishNet;
using UnityEngine;
using TMPro;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameFieldInput;
    [SerializeField] private TMP_InputField _ipAddressInput;
    private string ipAdress = null;
    private string playerName = null;

    public void OnEditName()
    {
        playerName = _nameFieldInput.text;
    }
    public void OnEditIpAdress()
    {
        // Check if the given Input is an ipAdress
        // Set the current ipAdress
        ipAdress = _ipAddressInput.text;
    }
    public void OnClickHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        // let the Save Manager check for the given Name and save or load accordingly
        GameSystem.Instance.SetGameState(GameState.Lobby);
    }
    public void OnClickJoin()
    {
        // let the Save Manager check for the given Name and sav or load accordingly
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;

        InstanceFinder.ClientManager.StartConnection(ip);
    }
    public void OnClickLeaveGame()
    {
        // CleanUp everything and close the game

    }
}
