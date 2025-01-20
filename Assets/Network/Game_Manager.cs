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
                // ������� ���� ���� ����� ������� Shutdown
                StartCoroutine(ShutdownAndLoadScene(name));
            }
        }
        else
        {
            Debug.Log("������� �������� �� ��� �������");
            SceneManager.LoadScene(name);
        }
    }

    private IEnumerator ShutdownAndLoadScene(string name)
    {
        // ���� ���� ����, ����� ��������� ��� ������� ������� ��������
        yield return null;
        NetworkManager.Singleton.Shutdown();
        yield return new WaitForSecondsRealtime(0.1f);
        Debug.Log("������� �������� ��������");
        SceneManager.LoadScene(name);
        yield return null;
    }


}
