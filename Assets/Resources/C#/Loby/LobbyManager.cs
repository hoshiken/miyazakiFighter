using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : NetworkBehaviour
{
    [Header("UI参照")]
    [SerializeField] private TMP_Text totalPlayersText;   // 対戦部屋 0人/10人
    [SerializeField] private TMP_Text[] roomCountTexts;   // 部屋1〜5 の人数テキスト

    [Header("設定")]
    public int maxPlayers = 10;
    private const int MaxRooms = 5; // 部屋数
    private const int RoomCapacity = 2; // 各部屋の上限人数

    // =======================
    // 同期されるサーバーデータ
    // =======================
    [SyncVar(hook = nameof(OnPlayerCountChanged))] private int connectedPlayers = 0;
    [SyncVar(hook = nameof(OnRoomCountsChanged))] private string roomCountsSerialized = "0,0,0,0,0";

    private int[] roomCounts = new int[MaxRooms];

    // =======================
    // 初期化・イベント購読
    // =======================
    private void Start()
    {
        if (isServer)
        {
            NetworkServer.OnConnectedEvent += OnServerClientConnected;
            NetworkServer.OnDisconnectedEvent += OnServerClientDisconnected;

            // Host起動時は自分をカウント
            if (isClient)
            {
                connectedPlayers = 1;
                UpdateTotalCount();
            }
        }
    }

    private void OnDestroy()
    {
        if (isServer)
        {
            NetworkServer.OnConnectedEvent -= OnServerClientConnected;
            NetworkServer.OnDisconnectedEvent -= OnServerClientDisconnected;
        }
    }

    // =======================
    // Mirrorの接続イベント
    // =======================
    private void OnServerClientConnected(NetworkConnectionToClient conn)
    {
        connectedPlayers++;
        UpdateTotalCount();
    }

    private void OnServerClientDisconnected(NetworkConnectionToClient conn)
    {
        connectedPlayers--;
        UpdateTotalCount();
    }

    // =======================
    // クライアント側からの部屋参加
    // =======================
    public void OnClickJoinRoom(int roomIndex)
    {
        if (NetworkClient.isConnected)
        {
            CmdJoinRoom(roomIndex);
        }
        else
        {
            Debug.Log("サーバーに接続されていません");
        }
    }

    // サーバー処理
    [Command(requiresAuthority = false)]
    void CmdJoinRoom(int roomIndex, NetworkConnectionToClient sender = null)
    {
        if (roomIndex < 0 || roomIndex >= MaxRooms) return;

        if (roomCounts[roomIndex] >= RoomCapacity)
        {
            Debug.Log($"部屋{roomIndex + 1}は満員です");
            return;
        }

        roomCounts[roomIndex]++;
        UpdateRoomCounts();

        Debug.Log($"プレイヤー{sender.connectionId}が部屋{roomIndex + 1}に参加");

        // キャラ選択画面へ
        TargetGoToCharacterSelect(sender, roomIndex);
    }

    // クライアント遷移
    [TargetRpc]
    void TargetGoToCharacterSelect(NetworkConnectionToClient target, int roomIndex)
    {
        Debug.Log($"部屋{roomIndex + 1}に入りました。キャラ選択シーンへ移動");
        SceneManager.LoadScene("CharacterSelectScene");
    }

    // =======================
    // HookでUI更新
    // =======================
    void OnPlayerCountChanged(int _, int newCount)
    {
        UpdateTotalCount();
    }

    void OnRoomCountsChanged(string _, string newValue)
    {
        UpdateRoomUI(newValue);
    }

    // =======================
    // サーバーでカウント更新
    // =======================
    [Server]
    private void UpdateTotalCount()
    {
        if (totalPlayersText != null)
            totalPlayersText.text = $"{connectedPlayers}人 / {maxPlayers}人";
    }

    [Server]
    private void UpdateRoomCounts()
    {
        roomCountsSerialized = string.Join(",", roomCounts);
        RpcUpdateRoomCounts(roomCountsSerialized);
    }

    [ClientRpc]
    void RpcUpdateRoomCounts(string serialized)
    {
        UpdateRoomUI(serialized);
    }

    // =======================
    // クライアントでUI反映
    // =======================
    private void UpdateRoomUI(string serialized)
    {
        string[] parts = serialized.Split(',');
        for (int i = 0; i < Mathf.Min(parts.Length, roomCountTexts.Length); i++)
        {
            roomCountTexts[i].text = $"{parts[i]}人 / {RoomCapacity}人";
        }
    }
}
