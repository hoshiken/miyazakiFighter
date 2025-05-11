using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineSetting : MonoBehaviourPunCallbacks
{
    private GameObject player;
    private Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        Connect("1.0");
    }

    private void Connect(string gameVersion)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /*
     * Callbacks
     */
    
    // Photonに接続
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    // Photonから切断された時
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause.ToString());
    }

    // マスターサーバーに接続した時
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinRandomRoom();
    }

    // ランダムな部屋への入室に失敗した時
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed: " + message);

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // 部屋に入室した時
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        if (PhotonNetwork.IsMasterClient)
        {
            position = new Vector3(-5f, 0f, 0f);
            player = PhotonNetwork.Instantiate("MasterPlayer", position, Quaternion.identity);

            Vector3 scale = player.transform.localScale;
            scale = new Vector3(-scale.x, scale.y, scale.z);
            player.transform.localScale = scale;
        }
        else
        {
            position = new Vector3(5f, 0f, 0f);
            player = PhotonNetwork.Instantiate("ClientPlayer", position, Quaternion.identity);
        }
    }
}