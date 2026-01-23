using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayerPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameDisplay;
    [SerializeField] private TMP_Text _playerScoreDisplay;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerShieldBar;

    private int _myPlayerId;

    public void Initialize(PlayerSessionData data)
    {
        _myPlayerId = data.PlayerID;
        _playerNameDisplay.text = data.PlayerName;
        _playerScoreDisplay.text = data.PlayerScore.ToString();
        GameEvents.OnPlayerStatsChanged.AddListener(UpdatePlayerUI);
    }

    private void UpdatePlayerUI(PlayerSession session, SpacecraftStats stats)
    {
        if (session.Owner.ClientId != _myPlayerId) return;

        _playerScoreDisplay.text = session.PlayerScore.Value.ToString();

        _playerHealthBar.fillAmount = stats.CurrentHealth / stats.MaxHealth;
        _playerShieldBar.fillAmount = stats.CurrentShield / stats.MaxShield;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerStatsChanged.RemoveListener(UpdatePlayerUI);
    }
}
