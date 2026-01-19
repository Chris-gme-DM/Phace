using FishNet;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField ipAdressInputField;
    private string ipAdress;
    private string playerName;

    public void OnEditName()
    {
        playerName =  playerNameInputField.text;
        Debug.Log($"Player: {playerName} would like to exist");
    }
    public void OnEditIpAdress()
    {
        ipAdress = ipAdressInputField.text;
        Debug.Log($"This player would like to join Lobby on ip Adress: {ipAdress}");
    } 

    public void OnClickHost()
    {
        GameSystem.Instance.SetActiveProfile(playerName);
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        OwnLobbyManager.Instance.RequestJoinLobby(GameSystem.Instance.ActiveProfile);
    }

    public void OnClickJoin()
    {
        // Hate on manual assignment of OnEditName, because reasons
        GameSystem.Instance.SetActiveProfile(playerName);
        // let the Save Manager check for the given Name and sav or load accordingly
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;
        InstanceFinder.ClientManager.StartConnection(ip);
        OwnLobbyManager.Instance.RequestJoinLobby(GameSystem.Instance.ActiveProfile);
    }
    public void OnClickLeaveGame()
    {
        // CleanUp everything and close the game
        InstanceFinder.ClientManager.StopConnection();
        Application.Quit();
    }
}
