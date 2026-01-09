using UnityEngine;
using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using FishNet.Object;
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string SceneName = "GameScene_0 1";

    [SerializeField] private NetworkObject gameManagerPrefab;
    [SerializeField] private NetworkObject lobbyManagerPrefab;
    // Only called once. Loads the main scene after bootstrapper is done readying the Managers.
    private IEnumerator Start()
    {
        // Start the Network
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
        // Wait until server is started
        while (!InstanceFinder.ServerManager.Started) yield return null;
        // Instantiate the Lobby and Game Managers from the Network Prefab Manager
        SpawnManagers();
        LoadMenu();
    }
    private void SpawnManagers()
    {
        if (lobbyManagerPrefab != null)
        {
            NetworkObject lobbyManager = Instantiate(lobbyManagerPrefab);
            InstanceFinder.ServerManager.Spawn(lobbyManager);
        }
        if (gameManagerPrefab != null)
        {
            NetworkObject gameManager = Instantiate(gameManagerPrefab);
            InstanceFinder.ServerManager.Spawn(gameManager);
        }
    }
    private void LoadMenu()
    {
        SceneLoadData sld = new (SceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}
