using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OnlineSetting : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerCountText;

    private GameObject player;
    private Vector3 position;
    private const string gameVersion = "1.0";
    private string currentRoomName;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true; // シーンの自動同期を有効化
    }

    void Start()
    {
        ConnectToPhoton();
        InvokeRepeating(nameof(UpdatePlayerCountUI), 1f, 1f);
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void UpdatePlayerCountUI()
    {
        // ロビーUIに playerCountText が存在している時のみ更新
        if (playerCountText != null && PhotonNetwork.InLobby)
        {
            int playerCount = PhotonNetwork.CountOfPlayers;
            playerCountText.text = $"{playerCount}人 / 20人";
        }
    }

    // ========= Callbacks ==========

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby(); // ロビーに入室
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("切断されました: " + cause.ToString());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("部屋に入りました: " + PhotonNetwork.CurrentRoom.Name);

        // シーンがまだ切り替わってない場合はここでロード
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            SceneManager.LoadScene("BattleScene");
        }

        // プレイヤー生成は OnSceneLoaded で行う
    }

    public override void OnLeftRoom()
    {
        Debug.Log("部屋を退出しました");
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("部屋の作成に失敗: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("部屋への入室に失敗: " + message);
    }

    // ========= 外部UIから呼び出す用のメソッド ==========

    public void JoinBattleRoom(string roomName)
    {
        currentRoomName = roomName;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public void LeaveCurrentRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // ========= プレイヤー生成をシーン読み込み後に =========
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene" && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            position = new Vector3(-5f, 0f, 0f);
            player = PhotonNetwork.Instantiate("MasterPlayer", position, Quaternion.identity);

            Vector3 scale = player.transform.localScale;
            scale = new Vector3(-scale.x, scale.y, scale.z);
            player.transform.localScale = scale;
        }
        else
        {
            position = new Vector3(5f, 0f, 0f);
            player = PhotonNetwork.Instantiate("ClientPlayer", position, Quaternion.identity);
        }
    }
}
