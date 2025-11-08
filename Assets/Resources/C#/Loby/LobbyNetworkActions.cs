using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyNetworkActions : NetworkBehaviour
{
    public static LobbyNetworkActions Instance;

    private void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [TargetRpc]
    public void TargetGoCharacterSelect(NetworkConnectionToClient conn)
    {
        SceneManager.LoadScene("CharacterSelect");
    }
}
