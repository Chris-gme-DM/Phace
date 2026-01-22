using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject _bossName;
    [SerializeField] private Image _bossHealthBar;

    private readonly TMP_Text BossNameText;
    private void OnEnable()
    {
        GameEvents.OnBossStatChanged.AddListener(UpdateBossPanelUI);
    }
    public void SetBossPanelUI()
    {
        BossNameText.text = _bossName.GetComponentInChildren<Text>().text.ToString();
        BossNameText.text = GameManager.Instance.ActiveBoss.SpacecraftData.SpacecraftName;
    }
    public void UpdateBossPanelUI(SpacecraftStats stats)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        var bossVar = GameManager.Instance.ActiveBoss.Stats.Value;
        _bossHealthBar.fillAmount = bossVar.CurrentHealth / bossVar.MaxHealth;
    }
    private void OnDisable()
    {
        GameEvents.OnBossStatChanged.RemoveAllListeners();
    }
}
