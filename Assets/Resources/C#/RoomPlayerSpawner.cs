using UnityEngine;
using Photon.Pun;

public class RoomPlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogWarning("Not in a Photon room. Cannot spawn player.");
            return;
        }

        Vector3 spawnPos;
        string prefabName;

        if (PhotonNetwork.IsMasterClient)
        {
            spawnPos = new Vector3(-5f, 0f, 0f);
            prefabName = "MasterPlayer";
        }
        else
        {
            spawnPos = new Vector3(5f, 0f, 0f);
            prefabName = "ClientPlayer";
        }

        GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);

        // マスターのスケール反転（オプション）
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 scale = player.transform.localScale;
            scale.x *= -1f;
            player.transform.localScale = scale;
        }
    }
}
