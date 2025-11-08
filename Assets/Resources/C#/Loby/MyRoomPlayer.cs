using Mirror;
using UnityEngine;

public class MyRoomPlayer : NetworkRoomPlayer
{
    // UIの部屋ボタンから呼ぶ
    public void JoinRoom(int index)
    {
        if (isLocalPlayer) CmdJoinRoom(index);
    }

    [Command]
    void CmdJoinRoom(int index, NetworkConnectionToClient sender = null)
    {
        var mgr = (MyRoomManager)NetworkManager.singleton;
        if (mgr.TryJoinRoom(sender, index))
        {
            // ここではゲームシーン遷移は行わず、まずは“入室カウント”だけを安定動作
            // 次段で CharacterSelect へ切り替える場合は:
            // mgr.ServerChangeScene(mgr.GameplayScene);
        }
    }
}
