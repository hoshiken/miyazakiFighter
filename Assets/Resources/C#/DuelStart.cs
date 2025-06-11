using Photon.Pun;
using UnityEngine;

public class DuelStart : MonoBehaviour
{
    public void OnClickStartBattle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleScene");  // 全員自動で遷移
        }
        else
        {
            Debug.LogWarning("バトル開始はマスタークライアントのみが実行できます");
        }
    }
}
