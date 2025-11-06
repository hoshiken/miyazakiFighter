using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    // クライアントが部屋を選んだとき
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

    // サーバーで処理
    [Command]
    void CmdJoinRoom(int roomIndex, NetworkConnectionToClient sender = null)
    {
        Debug.Log($"プレイヤー{sender.connectionId}が部屋{roomIndex}に参加");
        TargetGoToCharacterSelect(sender, roomIndex);
    }

    // クライアント個別に呼ばれる
    [TargetRpc]
    void TargetGoToCharacterSelect(NetworkConnectionToClient target, int roomIndex)
    {
        Debug.Log($"部屋{roomIndex}に入りました。キャラ選択シーンへ移動");
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
