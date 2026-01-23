using FishNet;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

public class PlayerSession : NetworkBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    public readonly SyncVar<string> PlayerName = new("Player");
    public readonly SyncVar<int> SpacecraftID = new(101);
    public readonly SyncVar<int> PlayerScore = new(0);
    public readonly SyncVar<bool> IsReady = new(false);
    [AllowMutableSyncType]
    public readonly SyncVar<NetworkObject> _controlledSpacecraft;

    public NetworkObject ControlledSC => _controlledSpacecraft.Value;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            StartCoroutine(DelayedLobbyJoin());
        }
    }
    private IEnumerator DelayedLobbyJoin()
    {
        yield return _waitForSeconds0_5;
        while (!InstanceFinder.ClientManager.Started) yield return null;

        PlayerProfile profile = GameSystem.Instance.ActiveProfile;
        if (profile != null)
        {
            PlayerSessionData data = new() 
            {
                PlayerName = profile.PlayerName,
                SpacecraftID = profile.SelectedSpacecraftID,
                IsReady = false
            };
            OwnLobbyManager.Instance.RpcRequestProfileUpdate(data);
        }
        GameEvents.ChangeGameState(GameState.Lobby);
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
        OwnLobbyManager.Instance.LobbyPlayers[Owner.ClientId] = GetSnapshot();
        OwnLobbyManager.Instance.OnPlayerReadyStatusChanged();
    }

    public void SetControlledSpacecraft(NetworkObject spacecraftNO)
    {
        _controlledSpacecraft.Value = spacecraftNO;
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