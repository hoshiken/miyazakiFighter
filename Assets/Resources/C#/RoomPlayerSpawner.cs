using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class RoomPlayerSpawner : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class CharPrefabEntry
    {
        public string name;
        public GameObject prefab;
    }
    public List<CharPrefabEntry> charPrefabs;

    public Vector3 defaultMasterPos = new Vector3(-5,0,0);
    public Vector3 defaultClientPos = new Vector3(5,0,0);

    private bool hasSpawned;

    void Start()
    {
        if (!PhotonNetwork.InRoom || hasSpawned) return;

        var props = PhotonNetwork.LocalPlayer.CustomProperties;
        string charName = props.ContainsKey("SelectedCharacter") ? props["SelectedCharacter"].ToString() : "Default";

        var entry = charPrefabs.Find(e => e.name == charName);
        if (entry == null) Debug.LogError($"Prefab not found for {charName}");

        Vector3 spawnPos = PhotonNetwork.IsMasterClient ? defaultMasterPos : defaultClientPos;
        var playerObj = PhotonNetwork.Instantiate(entry.prefab.name, spawnPos, Quaternion.identity);
        if (!PhotonNetwork.IsMasterClient)
        {
            var s = playerObj.transform.localScale;
            s.x *= -1f;
            playerObj.transform.localScale = s;
        }
        hasSpawned = true;
    }
}
