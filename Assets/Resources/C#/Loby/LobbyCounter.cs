using Mirror;
using TMPro;
using UnityEngine;

public class LobbyCounter : NetworkBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text totalPlayersText;
    [SerializeField] private TMP_Text[] roomTexts;

    private const int MaxRooms = 5;
    private const int RoomCapacity = 2;

    [SyncVar(hook = nameof(OnTotalChanged))] private int totalPlayers = 0;
    public readonly SyncList<int> roomCounts = new();

    public override void OnStartServer()
    {
        base.OnStartServer();
        roomCounts.Clear();
        for (int i = 0; i < MaxRooms; i++)
            roomCounts.Add(0);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        roomCounts.Callback += OnRoomCountChanged;
        UpdateAllUI();
    }

    public override void OnStopClient()
    {
        roomCounts.Callback -= OnRoomCountChanged;
        base.OnStopClient();
    }

    // サーバー専用メソッド
    [Server]
    public void ServerAddTotal(int delta)
    {
        totalPlayers = Mathf.Max(0, totalPlayers + delta);
    }

    [Server]
    public void ServerAddRoom(int idx, int delta)
    {
        roomCounts[idx] = Mathf.Max(0, roomCounts[idx] + delta);
    }

    [Server]
    public int GetRoomCount(int idx) => roomCounts[idx];

    // --- Hooks ---
    private void OnTotalChanged(int oldValue, int newValue)
    {
        UpdateAllUI();
    }

    // ✅ Mirror v96対応版（引数4つ）
    private void OnRoomCountChanged(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        if (totalPlayersText != null)
            totalPlayersText.text = $"{totalPlayers}人/10人";

        for (int i = 0; i < Mathf.Min(roomTexts.Length, roomCounts.Count); i++)
        {
            if (roomTexts[i] != null)
                roomTexts[i].text = $"{roomCounts[i]}人/{RoomCapacity}人";
        }
    }
}
