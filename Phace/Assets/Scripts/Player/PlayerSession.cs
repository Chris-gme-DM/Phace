using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;

public class PlayerSession : NetworkBehaviour
{
    public readonly SyncVar<string> PlayerName = new("Player");
    public readonly SyncVar<int> SpacecraftID = new();
    public readonly SyncVar<int> PlayerScore = new(0);
    public readonly SyncVar<bool> IsReady = new(false);

    private NetworkObject _controlledSpacecraft;
    public void SetFromProfile(PlayerProfile profile)
    {
        PlayerName.Value = profile.PlayerName;
        SpacecraftID.Value = profile.SelectedSpacecraftID;
        PlayerScore.Value = 0;          // New PlayerSessions have a score of 0
        IsReady.Value = false;          // Assume players are not ready yet
    }
    public PlayerSessionData GetSnapshot()
    {
        return new PlayerSessionData
        {
            PlayerID = (Owner != null) ? Owner.ClientId : -1,
            PlayerName = PlayerName.Value,
            SpacecraftID = SpacecraftID.Value,
            PlayerScore = PlayerScore.Value,
            IsReady = IsReady.Value,
        };
    }

    [ServerRpc]
    public void SetReadyStatus(bool ready)
    {
        IsReady.Value = ready;

        GameEvents.OnPlayerStatusChanged.Invoke(GetSnapshot());
    }

    public void SetControlledSpacecraft(NetworkObject spacecraftNO)
    {
        _controlledSpacecraft = spacecraftNO;
    }
}

[Serializable]
public struct PlayerSessionData
{
    public int PlayerID;
    public string PlayerName;
    public int SpacecraftID;
    public int PlayerScore;
    public bool IsReady;
}
