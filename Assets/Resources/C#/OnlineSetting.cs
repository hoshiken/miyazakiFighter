using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineSetting : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerCountText;

    private GameObject player;
    private Vector3 position;
    private const string gameVersion = "1.0";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
        if (playerCountText != null)
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
        // UI上で部屋ボタンを有効化するなどの処理
        // 例: UIManager.Instance.ShowRoomButtons();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("切断されました: " + cause.ToString());
    }

    public override void OnJoinedRoom()
{
    Debug.Log("部屋に入りました: " + PhotonNetwork.CurrentRoom.Name);

    // 対戦用シーンへ遷移（Room1～Room10）前提
    if (!PhotonNetwork.CurrentRoom.Name.StartsWith("Room"))
        return;

    UnityEngine.SceneManagement.SceneManager.LoadScene(PhotonNetwork.CurrentRoom.Name);
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
}
