using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class UIManager : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds3 = new(3f);
    #region Settings
    public static UIManager Instance { get; private set; }
    [Header("UI Panels")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _backgroundPanel;
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private GameObject _countDownObject;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _postGamePanel;
    public GameObject StartButton => _startButton;

    private PlayerInput _playerInput;
    private GameState CurrentGameState;
    private TMP_Text _countDownText;
    private bool IsAnyMenuOpen => _optionsPanel.activeSelf || _mainMenuPanel.activeSelf || _lobbyPanel.activeSelf;
    #endregion
    #region Initialization
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("UI");
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChange);
        GameEvents.OnPlayerStatusChanged.AddListener(HandlePlayerLobbyStatus);

        _countDownText = _countDownObject.GetComponent<TMP_Text>();

    }
    private void OnDisable()
    {
        GameEvents.OnGameStateChanged.RemoveListener(HandleGameStateChange);
        GameEvents.OnPlayerStatusChanged.RemoveListener(HandlePlayerLobbyStatus);
    }
    #endregion
    #region Event Handlers
    private void HandleGameStateChange(GameState newState)
    {
        CurrentGameState = newState;
        _gamePanel.SetActive(newState == GameState.InGame || newState == GameState.PostGame);
        _mainMenuPanel.SetActive(newState == GameState.MainMenu);
        _lobbyPanel.SetActive(newState == GameState.Lobby);
        _postGamePanel.SetActive(newState == GameState.PostGame);
        UpdateInputFocus();
        Debug.Log($"{CurrentGameState}");
    }
    private void HandlePlayerLobbyStatus(PlayerSessionData playerSessionData)
    {
        // Update lobby UI based on player status
        
    }
    private void UpdateInputFocus()
    {
        if (CurrentGameState == GameState.InGame && !IsAnyMenuOpen)
        {
            _playerInput.SwitchCurrentActionMap("SpaceshipTopDown");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }
    }
    #endregion
    #region Public Methods
    public void ToggleOptions()
    {
        bool isActive = !_optionsPanel.activeSelf;
        _optionsPanel.SetActive(isActive);
        UpdateInputFocus();
    }
    public IEnumerator ShowLoadingScreen()
    {
        _loadingPanel.SetActive(true);
        // Wait for a few seconds to simulate loading
        yield return _waitForSeconds3;
        _loadingPanel.SetActive(false);
    }
    public void RequestStartGame()
    {
        OwnLobbyManager.Instance.RpcRequestStartGame();
    }

    public IEnumerator CountDown(int seconds)
    {
        _countDownObject.SetActive(true);
        while (seconds > 0)
        {
            _countDownText.text = seconds.ToString();
            float elapsed = 0f;
            float duration = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float scale = Mathf.Lerp(2.5f, 0.5f, progress);
                _countDownText.transform.localScale = new Vector3(scale, scale, 1);
                _countDownText.alpha = Mathf.Lerp(1f, 0f, progress);

                yield return null;
            }
            seconds--;
        }
        _countDownObject.SetActive(false);
    }
    #endregion
}
