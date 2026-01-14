using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
public class Bootstrapper : MonoBehaviour
{
    [Header("Scene to Load")]
    [SerializeField] private string sceneName = "MainMenu_1";

    [Header("Networked Managers")]
    [SerializeField] private List<NetworkObject> networkedManagersPrefabs;

    [Header("Local Managers")]
    [SerializeField] private List<GameObject> localManagerPrefabs;
    // Only called once. Loads the main scene after bootstrapper is done readying the Managers.
    private IEnumerator Start()
    {
        SpawnAllManagers();
        LoadMainMenu();
        yield break;
    }
    private void SpawnAllManagers()
    {
        // Instantiate local Managers
        foreach (var localPrefab in localManagerPrefabs)
        {
            if (localPrefab == null) continue;
            GameObject go = Instantiate(localPrefab);
            DontDestroyOnLoad(go);

            // Instantiate networked Managers if Server is started.
            if (InstanceFinder.IsServerStarted)
            {
                foreach (var netPrefab in networkedManagersPrefabs)
                {
                    if (netPrefab == null) continue;
                    NetworkObject no = Instantiate(netPrefab);
                    InstanceFinder.ServerManager.Spawn(no);
                }
            }
        }
    }
    public void LoadMainMenu()
    {
        // Unity Scene Manager. Load Main Menu Scene
        SceneManager.LoadScene(sceneName);
        GameEvents.ChangeGameState(GameState.MainMenu);
        UIManager.Instance.ShowLoadingScreen();
    }
}
