using FishNet;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class MainMenuUI : MonoBehaviour
{
    private string ipAdress = null;
    private string playerName = null;

    public void OnEditName(string text) => playerName = text;
    public void OnEditIpAdress(string IPAdress) => ipAdress = IPAdress;

    public void OnClickHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        StartCoroutine(WaitAndJoinLobby());
    }

    public void OnClickJoin()
    {
        // let the Save Manager check for the given Name and sav or load accordingly
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;
        InstanceFinder.ClientManager.StartConnection(ip);
        StartCoroutine(WaitAndJoinLobby());
    }
    public void OnClickLeaveGame()
    {
        // CleanUp everything and close the game
        InstanceFinder.ClientManager.StopConnection();
        Application.Quit();
    }
    private IEnumerator WaitAndJoinLobby()
    {
        while (LobbyManager.Instance == null || !InstanceFinder.ClientManager.Started)
        { yield return null; }

        PlayerProfile profile = SaveManager.Instance.LoadPlayerProfile();

        if (!string.IsNullOrWhiteSpace(playerName)) profile.PlayerName = playerName;

        var summary = new PlayerSummary
        {
            PlayerName = profile.PlayerName,
            SpacecraftID = profile.SelectedSpacecraftID
        };

        LobbyManager.Instance.RequestJoinLobby(summary);
    }
}
