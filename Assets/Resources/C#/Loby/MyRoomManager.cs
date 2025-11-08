using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class MyRoomManager : NetworkRoomManager
{
    [Header("Lobby Counter Reference")]
    public LobbyCounter lobbyCounter;

    private const int MaxRooms = 5;
    private const int RoomCapacity = 2;

    private readonly Dictionary<int, int> connToRoom = new();

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        lobbyCounter.ServerAddTotal(1);
    }

    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);
        lobbyCounter.ServerAddTotal(-1);

        if (connToRoom.TryGetValue(conn.connectionId, out int room))
        {
            lobbyCounter.ServerAddRoom(room, -1);
            connToRoom.Remove(conn.connectionId);
        }
    }

    [Server]
    public void ServerJoinRoom(NetworkConnectionToClient conn, int index)
    {
        if (index < 0 || index >= MaxRooms) return;
        if (lobbyCounter.GetRoomCount(index) >= RoomCapacity)
        {
            Debug.Log($"Room {index + 1} is full.");
            return;
        }

        lobbyCounter.ServerAddRoom(index, 1);
        connToRoom[conn.connectionId] = index;

        // ★ここでNetworkBehaviour経由でRPCを実行
        LobbyNetworkActions.Instance.TargetGoCharacterSelect(conn);
    }
}
