using FishNet;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField _ipAddressInput;
    public void OnClickHost()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }
    public void OnClickJoin(string ipAdress)
    {
        string ip = string.IsNullOrEmpty(ipAdress) ? "localhost" : ipAdress;

        InstanceFinder.ClientManager.StartConnection(ip);
    }
}
