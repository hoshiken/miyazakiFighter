using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject masterPrefab;
    public GameObject clientPrefab;

    private void Start()
    {
        Vector3 spawnPos;
        GameObject prefabToSpawn;

        if (PhotonNetwork.IsMasterClient)
        {
            spawnPos = new Vector3(-5f, 0f, 0f);
            prefabToSpawn = masterPrefab;
        }
        else
        {
            spawnPos = new Vector3(5f, 0f, 0f);
            prefabToSpawn = clientPrefab;
        }

        PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPos, Quaternion.identity);
    }
}
