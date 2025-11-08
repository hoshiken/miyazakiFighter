using Mirror;
using UnityEngine;

public class MyRoomPlayer : NetworkRoomPlayer
{
    public void JoinRoom(int index)
    {
        if (isLocalPlayer)
            CmdJoinRoom(index);
    }

    [Command]
    private void CmdJoinRoom(int index)
    {
        var manager = (MyRoomManager)NetworkManager.singleton;
        manager.ServerJoinRoom(connectionToClient, index);
    }
}
