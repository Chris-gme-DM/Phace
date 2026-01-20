using FishNet;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

public class PlayerSession : NetworkBehaviour
{
    public readonly SyncVar<string> PlayerName = new("Player");
    public readonly SyncVar<int> SpacecraftID = new();
    public readonly SyncVar<int> PlayerScore = new(0);
    public readonly SyncVar<bool> IsReady = new(false);

    private NetworkObject _controlledSpacecraft;

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
        yield return new WaitForSeconds(0.5f);
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