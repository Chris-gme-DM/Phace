using FishNet;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField _ipAddressInput;
    private string ipAdress = null;

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
        GameSystem.Instance.SetGameState(GameState.Lobby);
    }
    public void OnClickJoin()
    {
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;

        InstanceFinder.ClientManager.StartConnection(ip);
    }
    public void OnClickLeaveGame()
    {
        // CleanUp everything and close the game

    }
}
