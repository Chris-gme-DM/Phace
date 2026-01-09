using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet;
public class BootstrapperManager : MonoBehaviour
{
    public static BootstrapperManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance !=  null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    // Update is called once per frame. forgot what i wanted to do here
    void Update()
    {
        if (!InstanceFinder.IsServerStarted)
            return;
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
