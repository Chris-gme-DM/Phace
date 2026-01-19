using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.Events;

public class GameInstance : BaseLobbyInstance
{
    private enum GameInstanceState
    {
        NotUsed,
        Used,
        Ready
    }
    public enum OnlineGameState
    {
        Lobby,
        Game,
        PostGame,
    }
    public UnityEvent GameStateReady = new();
    public UnityEvent OnlineGameStateChanged = new();
    // syncvar migration, was [SyncVar] private GameInstanceState _state;
    [AllowMutableSyncType] private SyncVar<GameInstanceState> _state = new();
    [AllowMutableSyncType] private SyncVar<OnlineGameState> _onlineState = new();
    /// <summary>
    /// SERVER ONLY:
    /// Marks the Game instance in use, the lobby will also be attached so the Game instance has the Lobby details.
    /// </summary>
    /// <param name="lobby">The Lobby that is using the Game Instance</param>
    [Server]
    public override void Use(Lobby lobby)
    {
        // Sets the lobby when game scene is used for a specific lobby
        base.Use(lobby);
        _state.Value = GameInstanceState.Used;
    }

    /// <summary>
    /// SERVER ONLY:
    /// Marks the Game instance ready, called upon when all networked clients are fully loaded in the scene and ready.
    /// </summary>
    [Server]
    public void Ready()
    {
        _state.Value = GameInstanceState.Ready;
        GameStateReady?.Invoke();
    }

    [Server]
    public void OnOnlineGameStateChanged(OnlineGameState state1, GameState state2)
    {
        _onlineState.Value = state1;
        GameEvents.ChangeGameState(state2);
    }
}