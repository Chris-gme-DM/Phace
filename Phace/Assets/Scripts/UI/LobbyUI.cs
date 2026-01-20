using FishNet.Object.Synchronizing;
using System;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUI : MonoBehaviour
{
    [Header("ScreenHalfs")]
    [SerializeField] private GameObject ScreenHalfUp;
    [SerializeField] private GameObject ScreenHalfDown;

    [Header("Configuration")]
    [SerializeField] private GameObject PlayerLobbyPanelPrefab;

    [SerializeField,] private LobbyPlayerPanelUI[] _playerPanels;

    // LobbyManager tells this how many players are present in the Lobby and to Update on Join or PlayerSetReady
    private void OnEnable()
    {
        if (OwnLobbyManager.Instance != null)
        {
            OwnLobbyManager.Instance.LobbyPlayers.OnChange += UpdateLobbyDisplay;
        }

    }
    private void UpdateLobbyDisplay(SyncDictionaryOperation op, int key, PlayerSessionData value, bool asServer)
    {
        // Redraw the player Panels
        foreach (var panel in _playerPanels)
        {
            panel.gameObject.SetActive(false);
        }
        int index = 0;

        foreach (var playerEntry in OwnLobbyManager.Instance.LobbyPlayers)
        {
            _playerPanels[index].gameObject.SetActive(true);
            _playerPanels[index].SetPlayerData(playerEntry.Value);
            index++;
        }
    }
    private void OnDisable()
    {
        OwnLobbyManager.Instance.LobbyPlayers.OnChange -= UpdateLobbyDisplay;
    }
}
