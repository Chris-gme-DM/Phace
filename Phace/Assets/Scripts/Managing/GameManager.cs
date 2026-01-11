using FishNet.Object;
using FishNet;
using UnityEngine;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SpacecraftData[] _spaceCraftDatas;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        Instance = this;
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        foreach (var session in LobbyManager.Instance.ActiveSessions.Values)
        {
            SpacecraftData data = System.Array.Find(_spaceCraftDatas, sc => sc.SpacecraftID == session.SpacecraftID.Value);
            if (data != null)
            {
                GameObject obj = Instantiate(data.SpacecraftPrefab);
            }
        }
    }
}