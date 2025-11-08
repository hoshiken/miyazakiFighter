using Mirror;
using UnityEngine;

public class ButtonJoinRoom : MonoBehaviour
{
    public int roomIndex;
    public void Click()
    {
        if (NetworkClient.localPlayer != null)
        {
            var player = NetworkClient.localPlayer.GetComponent<MyRoomPlayer>();
            if (player) player.JoinRoom(roomIndex);
        }
    }
}
