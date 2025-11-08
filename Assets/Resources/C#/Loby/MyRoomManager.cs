using Mirror;
using UnityEngine;

public class MyRoomManager : NetworkRoomManager
{
    [Header("Optional UI")]
    public LobbyCounter lobbyCounter;

    const int MaxRooms = 5;
    const int RoomCap  = 2;

    readonly System.Collections.Generic.Dictionary<int,int> connToRoom = new();

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);
        lobbyCounter?.ServerAddTotal(+1);
    }
    public override void OnRoomServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerDisconnect(conn);
        lobbyCounter?.ServerAddTotal(-1);

        if (connToRoom.TryGetValue(conn.connectionId, out int idx))
        {
            lobbyCounter?.ServerAddRoom(idx, -1);
            connToRoom.Remove(conn.connectionId);
        }
    }

    // Roomボタンから呼ばれる
    [Server]
    public bool TryJoinRoom(NetworkConnectionToClient conn, int roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= MaxRooms) return false;
        if (lobbyCounter != null && lobbyCounter.GetRoomCount(roomIndex) >= RoomCap) return false;

        lobbyCounter?.ServerAddRoom(roomIndex, +1);
        connToRoom[conn.connectionId] = roomIndex;
        return true;
    }
}
