using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineSetting : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI playerCountText;
    // Common battle scene name
    [SerializeField] private string SelectSceneName = "CharacterSelect"; // JoinRoom.cs と同じ名前を使用するか、共通の定数を持つスクリプトから参照する

    private GameObject player; // この変数はプレイヤーの生成ロジックをRoomPlayerSpawnerに移動したため、不要になる可能性があります。
    private Vector3 position; // この変数はプレイヤーの生成ロジックをRoomPlayerSpawnerに移動したため、不要になる可能性があります。
    private const string gameVersion = "1.0";

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // シーンの自動同期を有効にする（マスタークライアントがシーンをロードすると他のクライアントも同期）
        PhotonNetwork.AutomaticallySyncScene = true; // この行はAwakeかStartで一度だけ呼ぶのが推奨
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

        // マスタークライアントのみが共通のバトルシーンをロードし、他のクライアントは自動的に同期される
        if (PhotonNetwork.IsMasterClient)
        {
            // PhotonNetwork.LoadLevelを使用することで、シーンロードの同期をPhotonに任せる
            // 'SelectSceneName' に設定された共通のシーン名（例: "BattleScene"）をロード
            PhotonNetwork.LoadLevel(SelectSceneName);
        }
        // クライアント側はPhotonNetwork.AutomaticallySyncScene = true; の設定により、
        // マスタークライアントがシーンをロードした時点で自動的に同じシーンがロードされるため、
        // ここでクライアント側がSceneManager.LoadSceneを呼ぶ必要はありません。
        // もしOnJoinedRoomでクライアント側もシーン遷移したい場合は、
        // PhotonNetwork.CurrentRoom.PlayerCount == 2 といった条件で、
        // プレイヤーが揃ったタイミングでロードを呼ぶケースもありますが、
        // PhotonNetwork.AutomaticallySyncScene があれば不要なことが多いです。
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
