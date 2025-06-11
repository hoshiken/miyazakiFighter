using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private string roomName; // InspectorでRoom1〜Room10を指定
    [SerializeField] private string battleSceneName = "CharacterSelect"; // 実際に使う対戦シーン名

    public void OnClickJoinRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        SceneManager.LoadScene(battleSceneName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"Failed to join room {roomName}: {message}");
    }
}
