using UnityEngine;
using UnityEngine.InputSystem;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject loadingPanel;

    private PlayerInput _playerInput;
    private GameState CurrentGameState; // Since this follows the GameState that is set in the Lobby Manager
    private bool IsAnyMenuOpen => optionsPanel.activeSelf || mainMenuPanel.activeSelf || lobbyPanel.activeSelf;

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
    public void OnGameStateChanged(GameState newState)
    {
        CurrentGameState = newState;
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);

        switch (newState)
        {
            case GameState.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case GameState.Lobby:
                lobbyPanel.SetActive(true);
                break;
            case GameState.InGame:
                gamePanel.SetActive(true);
                break;
        }
        UpdateInputFocus();
    }
    public void UpdateInputFocus()
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
    public void ToggleOptions()
    {
        bool isActive = !optionsPanel.activeSelf;
        optionsPanel.SetActive(isActive);
        UpdateInputFocus();
    }
    public void OpenLobby()
    {

    }
    public void OpenMainMenu()
    {

    }
}
