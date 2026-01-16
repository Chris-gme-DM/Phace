using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BootstrapperManager : MonoBehaviour
{
    public static BootstrapperManager Instance { get; private set; }

    [Header("Scene to Load")]
    [SerializeField] private string sceneName = "MainMenu_1";

    [Header("Networked Managers")]
    [SerializeField] private List<NetworkObject> networkedManagersPrefabs;

    [Header("Local Managers")]
    [SerializeField] private List<GameObject> localManagerPrefabs;
    // Only called once. Loads the main scene after bootstrapper is done readying the Managers.
    private void Awake()
    {
        if (Instance !=  null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

    }
    private void Start()
    {

        StartCoroutine(LoadMainApp());
        SpawnAllManagers();
    }
    private void SpawnAllManagers()
    {
        // Instantiate local Managers
        foreach (var localPrefab in localManagerPrefabs)
        {
            if (localPrefab == null) continue;
            GameObject go = Instantiate(localPrefab);
            DontDestroyOnLoad(go);
            Debug.Log($"Manager {go} loaded");

        }
        // Instantiate networked Managers if Server is started.
        foreach (var netPrefab in networkedManagersPrefabs)
        {
            if (netPrefab == null) continue;
            NetworkObject no = Instantiate(netPrefab);
            InstanceFinder.ServerManager.Spawn(no);
            Debug.Log($"Manager {no} loaded");
        }
    }
    public void LoadMainMenu()
    {
        // Unity Scene Manager. Load Main Menu Scene
        SceneManager.LoadScene(sceneName);
    }
    public void OnMainMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != sceneName) return;
        SceneManager.sceneLoaded -= OnMainMenuLoaded;
        GameEvents.ChangeGameState(GameState.MainMenu);
    }

    private IEnumerator LoadMainApp()
    {
        // Wait until InstanceFinder actually sees the NetworkManager
        while (InstanceFinder.NetworkManager == null)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowLoadingScreen();
            yield return null;
        }

        // Now that it's found, move to the Main Menu
        SceneManager.sceneLoaded += OnMainMenuLoaded;
        SceneManager.LoadScene(sceneName);
    }
}
public static class PerformBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeBootstrapper()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}
