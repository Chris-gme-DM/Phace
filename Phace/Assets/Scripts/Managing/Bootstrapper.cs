using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
public class Bootstrapper : MonoBehaviour
{
    [Header("Scene to Load")]
    [SerializeField] private string sceneName = "Game_1";

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
    private void LoadMainMenu()
    {
        SceneLoadData sld = new (sceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}
