using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class holds all the behaviour relevant to the exact PlayerPanel.
/// </summary>
public class LobbyPlayerPanelUI : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TMP_Text playerNameDisplay;
    [SerializeField] private Image _spacecraftImage;
    [SerializeField] private TMP_Text _scName;
    [SerializeField] private Image _scHealth;
    [SerializeField] private Image _scShield;
    [SerializeField] private Image _scSpeed;
    [SerializeField] private Image _scPrimaryAttack;
    [SerializeField] private Image _scSecondaryAttack;
    [SerializeField] private GameObject _readyButton;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private GameObject _nextButton;

    private int _currentShipId;
    private bool _isReady;
    public void SetPlayerData(PlayerSessionData data)
    {
        if (playerNameDisplay != null)
            playerNameDisplay.text = data.PlayerName;

        _isReady = data.IsReady;

        SpacecraftData sc = GameSystem.Instance.GetSpacecraftDataById(data.SpacecraftID);
        if (sc != null)
        {
            if (sc.name != null) _scName.text = sc.name;
            _currentShipId = sc.SpacecraftID;
            if (sc.Icon != null) _spacecraftImage.sprite = sc.Icon;
            if (_scHealth != null) _scHealth.fillAmount = sc.BaseHealth / 200f;
            if (_scShield !=null) _scShield.fillAmount = sc.BaseShield / 100f;
            if (_scSpeed != null) _scSpeed.fillAmount = sc.BaseMaxSpeed / 10f;

 //           _scPrimaryAttack.sprite = sc.PrimaryAttack.AttackSprite;
   //         _scSecondaryAttack.sprite = sc.SecondaryAttack.AttackSprite;
        }

        bool isLocalPlayer = (data.PlayerID == InstanceFinder.ClientManager.Connection.ClientId);

        _readyButton.SetActive(isLocalPlayer);
        _prevButton.SetActive(isLocalPlayer);
        _nextButton.SetActive(isLocalPlayer);
        // Ready Button
        if (_readyButton.TryGetComponent<Image>(out var btnImage)) btnImage.color = _isReady ? Color.green : new Color(1f, 0.5f, 0f);
        TMP_Text btnText = _readyButton.GetComponentInChildren<TMP_Text>();
        if (btnText != null)
        {
            btnText.text = _isReady ? "READY!" : "READY?";
        }
        if (playerNameDisplay.TryGetComponent<Text>(out var nameText)) nameText.color = _isReady ? Color.green : new Color(1f, 0.5f, 0f);
    }
    // Set this in the fucking button
    public void OnClickPrev()
    {
        // Search the previous entry on the PlayerSpacecraft list
        int currentId = _currentShipId;
        int nextId = GameSystem.Instance.GetPrevPlayerShipId(currentId);
        RequestUpdate(nextId, false);
    }
    // Set this in the fucking button
    public void OnClickNext() 
    {
        int currentId = _currentShipId;
        int nextId = GameSystem.Instance.GetNextPlayerShipId(currentId);
        RequestUpdate(nextId, false);

    }
    public void OnClickReady()
    {
        RequestUpdate(_currentShipId, !_isReady);

    }
    private void RequestUpdate(int nextId, bool readyStatus)
    {
        PlayerSessionData updateData = new()
        {
            PlayerID = InstanceFinder.ClientManager.Connection.ClientId,
            PlayerName = GameSystem.Instance.ActiveProfile.PlayerName,
            SpacecraftID = nextId,
            IsReady = readyStatus,
        };
        OwnLobbyManager.Instance.RpcRequestProfileUpdate(updateData);
    }
}
