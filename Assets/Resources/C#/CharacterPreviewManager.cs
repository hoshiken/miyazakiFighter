using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterPreviewManager : MonoBehaviourPunCallbacks
{
    public Transform leftSpawnPoint;   // 1P用
    public Transform rightSpawnPoint;  // 2P用
    public GameObject[] characterPrefabs;

    private GameObject myPreview;
    private GameObject otherPreview;

    public void ShowCharacterPreview(Player player, string characterName)
    {
        bool isLocalPlayer = player == PhotonNetwork.LocalPlayer;
        bool isPlayer1 = player.IsMasterClient;

        Transform spawnPoint = isPlayer1 ? leftSpawnPoint : rightSpawnPoint;
        GameObject targetPrefab = GetCharacterPrefab(characterName);

        if (targetPrefab == null)
        {
            Debug.LogWarning($"キャラクタープレハブが見つかりません: {characterName}");
            return;
        }

        // 既存を削除
        if (isLocalPlayer)
        {
            if (myPreview != null) Destroy(myPreview);
        }
        else
        {
            if (otherPreview != null) Destroy(otherPreview);
        }

        // 生成
        GameObject preview = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
        Vector3 scale = Vector3.one * 1.5f;
        if (!isPlayer1) scale.x *= -1;  // 2Pは左右反転
        preview.transform.localScale = scale;

        if (isLocalPlayer)
            myPreview = preview;
        else
            otherPreview = preview;
    }

    private GameObject GetCharacterPrefab(string name)
    {
        foreach (var prefab in characterPrefabs)
        {
            if (prefab.name == name)
                return prefab;
        }
        return null;
    }

    // 相手がキャラを変更した時に呼ばれる
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("SelectedCharacter"))
        {
            string characterName = changedProps["SelectedCharacter"].ToString();
            ShowCharacterPreview(targetPlayer, characterName);
        }
    }
}
