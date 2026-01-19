using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    [Header("ScreenHalfs")]
    [SerializeField] private GameObject ScreenHalfUp;
    [SerializeField] private GameObject ScreenHalfDown;

    [Header("Configuration")]
    [SerializeField] private GameObject PlayerLobbyPanelPrefab;

    // LobbyManager tells this how many players are present in the Lobby and to Update on Join or PlayerSetReady
    private void OnEnable()
    {
        OwnLobbyManager.Instance.LobbyPlayers.OnChange += UpdateLobbyDisplay;
    }
    private void UpdateLobbyDisplay(SyncDictionaryOperation op, int key, PlayerSessionData value, bool asServer)
    {
        // Redraw the player Panels
    }
    private void OnDisable()
    {
        OwnLobbyManager.Instance.LobbyPlayers.OnChange -= UpdateLobbyDisplay;
    }
}
