using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerSession : NetworkBehaviour
{
    public readonly SyncVar<int> PlayerID = new();
    public readonly SyncVar<string> PlayerName = new();
    public readonly SyncVar<int> SpacecraftID = new();
    public readonly SyncVar<bool> IsReady = new();

    [ServerRpc]
    public void CmdInitialize(int playerID, string name, int spacecraftID)
    {
        PlayerID.Value = playerID;
        PlayerName.Value = name;
        SpacecraftID.Value = spacecraftID;
        IsReady.Value = false;
    }

    public void SetReadyStatus(bool prev, bool next, bool asServer)
    {
        GameEvents.OnPlayerStatusChanged.Invoke(new PlayerLobbyData {
            PlayerID = this.PlayerID.Value,
            PlayerName = this.PlayerName.Value,
            SelectedSpacecraftID = this.SpacecraftID.Value,
            IsReady = next
        });

    }
}
