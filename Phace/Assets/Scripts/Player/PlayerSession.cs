using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerSession : NetworkBehaviour
{
    public readonly SyncVar<string> PlayerName = new("Player");
    public readonly SyncVar<int> SpacecraftID = new(0);
    public readonly SyncVar<bool> IsReady = new(false);

    public void SetFromProfile(PlayerProfile profile)
    {
        PlayerName.Value = profile.PlayerName;
        SpacecraftID.Value = profile.SelectedSpacecraftID;
        IsReady.Value = false;
    }

    [ServerRpc]
    public void SetReadyStatus(bool prev, bool next, bool asServer)
    {
        GameEvents.OnPlayerStatusChanged.Invoke(new PlayerInfo
        {
            PlayerName = PlayerName.Value,
            SpacecraftID = SpacecraftID.Value,
            IsReady = next
        });

    }
}
