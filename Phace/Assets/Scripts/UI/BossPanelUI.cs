using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject _bossName;
    [SerializeField] private Image _bossHealthBar;

    private readonly TMP_Text _bossNameText;
    private void OnEnable()
    {
        GameEvents.OnBossStatChanged.AddListener(UpdateBossPanelUI);
        if (GameManager.Instance.ActiveBoss != null)
        {
            SetBossPanelUI();
            UpdateBossPanelUI(GameManager.Instance.ActiveBoss.Stats.Value);
        }
    }
    public void SetBossPanelUI()
    {
        if (GameManager.Instance.ActiveBoss != null && GameManager.Instance.ActiveBoss.SpacecraftData != null)
        {
            _bossNameText.text = GameManager.Instance.ActiveBoss.SpacecraftData.SpacecraftName;
        }
    }
    public void UpdateBossPanelUI(SpacecraftStats stats)
    {
        if (GameManager.Instance.ActiveBoss == null) return;
        //if (!gameObject.activeSelf) gameObject.SetActive(true);
        //var bossVar = GameManager.Instance.ActiveBoss.Stats.Value;
        _bossHealthBar.fillAmount = stats.CurrentHealth / stats.MaxHealth;
    }
    private void OnDisable()
    {
        GameEvents.OnBossStatChanged.RemoveAllListeners();
    }
}
