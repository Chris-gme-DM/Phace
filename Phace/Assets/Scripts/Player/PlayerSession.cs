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
        IsReady.Value = true;

        LobbyManager.Instance.CheckAllPlayersReady();
    }
}
