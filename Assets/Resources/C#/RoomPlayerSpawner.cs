using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomPlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 masterSpawnPos = new Vector3(-5f, 0f, 0f);
    [SerializeField] private Vector3 clientSpawnPos = new Vector3(5f, 0f, 0f);

    private bool hasSpawned = false;

    private void Start()
    {
        // 対戦シーンでのみスポーン処理
        if (PhotonNetwork.InRoom && SceneManager.GetActiveScene().name.StartsWith("Room"))
        {
            SpawnPlayerIfNeeded();
        }
    }

    void SpawnPlayerIfNeeded()
    {
        if (hasSpawned) return;

        if (PhotonNetwork.LocalPlayer.TagObject == null)
        {
            Vector3 spawnPos = PhotonNetwork.IsMasterClient ? masterSpawnPos : clientSpawnPos;
            GameObject spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
            PhotonNetwork.LocalPlayer.TagObject = spawnedPlayer;

            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 scale = spawnedPlayer.transform.localScale;
                scale.x *= -1f;
                spawnedPlayer.transform.localScale = scale;
            }

            hasSpawned = true;
        }
    }
}
