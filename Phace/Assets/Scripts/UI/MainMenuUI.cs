using FishNet;
using UnityEngine;
using TMPro;
using System.Collections;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameFieldInput;
    [SerializeField] private TMP_InputField _ipAddressInput;
    private string ipAdress = null;
    private string playerName = null;

    public void OnEditName() => playerName = _nameFieldInput.text;
    public void OnEditIpAdress() => ipAdress = _ipAddressInput.text;

    public void OnClickHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        StartCoroutine(WaitAndJoinLobby());

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

    }
}
