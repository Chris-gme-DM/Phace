using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePanelUI : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject _playerPanelPrefab;
    [SerializeField] private GameObject _bossPanel;

    [Header("LevelPanel")]
    [SerializeField] private GameObject _levelPanel;
    [SerializeField] private TMP_Text _levelText;

    private readonly List<GameObject> _spawnedPlayerPanels = new(); 
    private void Start()
    {
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChange);
        GameEvents.OnLevelChanged.AddListener(HandleLevelChange);
    }

    private void HandleLevelChange()
    {
        _levelText.text = GameManager.Instance.Level.ToString();
    }

    private void HandleGameStateChange(GameState arg0)
    {
        if (arg0 == GameState.InGame) 
        {
            CleanUp();

            int index = 0;
            // Check all active Players in the Lobby
            foreach (var session in OwnLobbyManager.Instance.LobbyPlayers.Values)
            {
                if (index >= 4) break;
                GameObject panelGo = Instantiate(_playerPanelPrefab);
                _spawnedPlayerPanels.Add(panelGo);

                RectTransform rt = panelGo.GetComponent<RectTransform>();

                Vector3 mirrorPos = rt.localPosition;
                if (index == 1 || index == 3) mirrorPos.x *= -1;
                if (index == 2 || index == 3) mirrorPos.y *= -1;

                rt.localPosition = mirrorPos;

                if (panelGo.TryGetComponent<GamePlayerPanelUI>(out var hud))
                {
                    hud.Initialize(session);
                }
                index++;
            }
        }
        else
        {
            CleanUp();
        }
    }

    private void CleanUp()
    {
        foreach (var panel in _spawnedPlayerPanels)
        { 
            if (panel != null) Destroy(panel);
        }
        _spawnedPlayerPanels.Clear();
        _bossPanel.SetActive(false);
    }

    private void PopulatePlayerPanel(PlayerSession session, int index)
    {


    }
    private void OnDisable()
    {
        GameEvents.OnGameStateChanged.RemoveAllListeners();
        GameEvents.OnLevelChanged.RemoveAllListeners();
    }
}
