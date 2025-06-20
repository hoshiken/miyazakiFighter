using Photon.Pun;
using UnityEngine;

public class BattleSceneManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return; // 同期処理はマスターだけが行う

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length != 2)
        {
            Debug.LogError("プレイヤーが2人いません");
            return;
        }

        // x位置で昇順ソート（左 → 右）
        System.Array.Sort(players, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // 左側 → 右向き（+1）
        PhotonView leftPV = players[0].GetComponent<PhotonView>();
        if (leftPV != null)
            leftPV.RPC("SetFacingDirection", RpcTarget.AllBuffered, 1f);

        // 右側 → 左向き（-1）
        PhotonView rightPV = players[1].GetComponent<PhotonView>();
        if (rightPV != null)
            rightPV.RPC("SetFacingDirection", RpcTarget.AllBuffered, -1f);
    }
}
