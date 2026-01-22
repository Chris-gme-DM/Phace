using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject _playerNameDisplay;
    [SerializeField] private GameObject _playerScoreDisplay;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerShieldBar;

    public void Initialize(PlayerSessionData data)
    {
        _playerNameDisplay.GetComponentInChildren<Text>().text = data.PlayerName;
        _playerScoreDisplay.GetComponentInChildren<Text>().text = data.PlayerScore.ToString();
        
        GameEvents.OnPlayerStatsChanged.AddListener(UpdatePlayerUI);
    }

    private void UpdatePlayerUI(PlayerSession session, SpacecraftStats stats)
    {
        _playerScoreDisplay.GetComponentInChildren<Text>().text = session.PlayerScore.ToString();
        _playerHealthBar.fillAmount = stats.CurrentHealth / stats.MaxHealth;
        _playerShieldBar.fillAmount = stats.CurrentShield / stats.MaxShield;
    }

}
