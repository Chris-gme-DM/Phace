using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBootstrapper : MonoBehaviour
{
    [SerializeField] private List<NetworkObject> networkManagerPrefabs; 
    private void Awake()
    {
        // Hook into the ServerManager state change event
        // We use InstanceFinder to find our local NetworkManager components
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerStateChanged;
    }

    private void OnDestroy()
    {
        // Always unhook events in OnDestroy to prevent memory leaks
        if (InstanceFinder.ServerManager != null)
            InstanceFinder.ServerManager.OnServerConnectionState -= OnServerStateChanged;
    }

    private void OnServerStateChanged(ServerConnectionStateArgs args)
    {
        // When the Server starts (either on Host or Dedicated)
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            SpawnNetworkManagers();
        }
    }

    private void SpawnNetworkManagers()
    {
        foreach (var netPrefab in networkManagerPrefabs)
        {
            if (netPrefab == null) continue;
            NetworkObject no = Instantiate(netPrefab);
            InstanceFinder.ServerManager.Spawn(no);
            // Add this item to the lsit of objects that should be moved

            Debug.Log($"Manager {no} loaded");
        }

    }
}