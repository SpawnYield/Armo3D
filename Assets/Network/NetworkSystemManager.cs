using Unity.Netcode;
using UnityEngine;

public class NetworkSystemManager : MonoBehaviour
{
    [SerializeField, Header("Offline Object")] private GameObject offlinePrefab;
    [SerializeField, Header("Online Object")] private GameObject onlinePrefab;

    public static GameObject CurrentMode { get; private set; }
    public static NetworkManager InstanceNetworkManager { get; private set; }

    public void StartOffline() => InitializeNetwork(false, CreateWorld);

    public void StartHost() => InitializeNetwork(true, CreateWorld);

    public void Connect() => InitializeNetwork(true, LoadWorld);

    private void InitializeNetwork(bool isOnline, System.Action postInitAction)
    {
        if (CurrentMode != null) Destroy(CurrentMode);

        CurrentMode = Instantiate(isOnline ? onlinePrefab : offlinePrefab);
        CurrentMode.TryGetComponent(out NetworkManager _NetworkManager);
        if (_NetworkManager == null)
        {
            Debug.LogError("NetworkManager not found in the instantiated object.");
            return;
        }
        else
        {
            InstanceNetworkManager = _NetworkManager;
        }

        DontDestroyOnLoad(CurrentMode);
        Debug.Log($"{(isOnline ? "Online" : "Offline")} mode initialized.");

        postInitAction?.Invoke();
    }

    private void CreateWorld()
    {
        InstanceNetworkManager.StartHost();
        ConfigureSceneManager();
        InstanceNetworkManager.SceneManager.LoadScene("-World-", UnityEngine.SceneManagement.LoadSceneMode.Single);
        Debug.Log("Hosting World...");
    }

    private void LoadWorld()
    {
        InstanceNetworkManager.StartClient();
        ConfigureSceneManager();
        Debug.Log("Connecting to World...");
        InstanceNetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void ConfigureSceneManager()
    {
        if (InstanceNetworkManager.SceneManager != null)
        {
            InstanceNetworkManager.SceneManager.ActiveSceneSynchronizationEnabled = true;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("Client connected with ID: " + clientId);
    }
}
