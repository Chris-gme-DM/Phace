using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayerPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameDisplay;
    [SerializeField] private TMP_Text _playerScoreDisplay;
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerShieldBar;

    public void Initialize(PlayerSessionData data)
    {
        _playerNameDisplay.text = data.PlayerName;
        _playerScoreDisplay.text = data.PlayerScore.ToString();
        
        GameEvents.OnPlayerStatsChanged.AddListener(UpdatePlayerUI);
    }

    private void UpdatePlayerUI(PlayerSession session, SpacecraftStats stats)
    {
        _playerScoreDisplay.text = session.PlayerScore.ToString();
        _playerHealthBar.fillAmount = stats.CurrentHealth / stats.MaxHealth;
        _playerShieldBar.fillAmount = stats.CurrentShield / stats.MaxShield;
    }

}
