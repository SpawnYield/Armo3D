using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : NetworkBehaviour
{
  
    public PlayerController playerController;
    public Move_Module _moveModule;
    public World_Blocks _blocks;
    public static Game_Manager instance;

    private void Awake()
    {
        instance = this;    
        Debug.Log("Started");
        _blocks.Init();
        playerController.Init();

    }
    public void DoToScene(string name)
    {
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient))
        {
            if (name == "+Menu+")
            {
                // Ожидаем один кадр перед вызовом Shutdown
                StartCoroutine(ShutdownAndLoadScene(name));
            }
        }
        else
        {
            Debug.Log("Сетевой менеджер не был активен");
            SceneManager.LoadScene(name);
        }
    }

    private IEnumerator ShutdownAndLoadScene(string name)
    {
        // Ждем один кадр, чтобы завершить все текущие сетевые операции
        yield return null;
        NetworkManager.Singleton.Shutdown();
        yield return new WaitForSecondsRealtime(0.1f);
        Debug.Log("Сетевой менеджер Отключен");
        SceneManager.LoadScene(name);
        yield return null;
    }


}
