using UnityEngine;
using UnityEngine.InputSystem;
public class UIManager : MonoBehaviour
{
    #region Settings
    public static UIManager Instance { get; private set; }
    [Header("UI Panels")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _backgroundPanel;
    [SerializeField] private GameObject _loadingPanel;

    private PlayerInput _playerInput;
    private GameState CurrentGameState;
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
    private void Start() => _playerInput = GetComponent<PlayerInput>();
    private void OnEnable()
    {
        GameEvents.OnGameStateChanged.AddListener(HandleGameStateChange);
        GameEvents.OnPlayerStatusChanged.AddListener(HandlePlayerLobbyStatus);
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
        _gamePanel.SetActive(newState == GameState.InGame);
        _mainMenuPanel.SetActive(newState == GameState.MainMenu);
        _lobbyPanel.SetActive(newState == GameState.Lobby || newState == GameState.PostGame); // If we make a post game panel, change this
        UpdateInputFocus();
    }
    private void HandlePlayerLobbyStatus(PlayerLobbyData playerLobbyData)
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
    public void ShowLoadingScreen()
    {
        _loadingPanel.SetActive(true);
        // Wait for a few seconds to simulate loading
        WaitForSecondsRealtime wait = new(3f);
        _loadingPanel.SetActive(false);
    }
    #endregion
}
