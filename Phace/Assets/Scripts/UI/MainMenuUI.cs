using FishNet;
using UnityEngine;
using TMPro;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField ipAdressInputField;
    private string ipAdress;
    private string playerName;

    public void OnEditName()
    {
        playerName =  playerNameInputField.text;
    }
    public void OnEditIpAdress()
    {
        ipAdress = ipAdressInputField.text;
    } 

    public void OnClickHost()
    {
        GameSystem.Instance.SetActiveProfile(playerName);
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void OnClickJoin()
    {
        // Hate on manual assignment of OnEditName, because reasons
        GameSystem.Instance.SetActiveProfile(playerName);
        // let the Save Manager check for the given Name and save or load accordingly
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;
        InstanceFinder.ClientManager.StartConnection(ip);
    }
    public void OnClickLeaveGame()
    {
        // CleanUp everything and close the game
        SaveManager.Instance.SavePlayerProfile(GameSystem.Instance.ActiveProfile);
        InstanceFinder.ClientManager.StopConnection();
        Application.Quit();
    }
}
