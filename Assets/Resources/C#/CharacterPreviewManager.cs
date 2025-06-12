using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterPreviewManager : MonoBehaviourPunCallbacks
{
    public Transform leftSpawnPoint;   // 1P用（MasterClient）
    public Transform rightSpawnPoint;  // 2P用
    public GameObject[] characterPrefabs;

    private GameObject leftPreview;    // 1P側に表示するキャラ
    private GameObject rightPreview;   // 2P側に表示するキャラ

    public void ShowCharacterPreview(Player player, string characterName)
    {
        bool isPlayer1 = player.IsMasterClient;
        Transform spawnPoint = isPlayer1 ? leftSpawnPoint : rightSpawnPoint;

        GameObject targetPrefab = GetCharacterPrefab(characterName);
        if (targetPrefab == null)
        {
            Debug.LogWarning($"キャラクタープレハブが見つかりません: {characterName}");
            return;
        }

        // 表示済みのキャラを削除
        if (isPlayer1)
        {
            if (leftPreview != null) Destroy(leftPreview);
        }
        else
        {
            if (rightPreview != null) Destroy(rightPreview);
        }

        // キャラ生成
        GameObject preview = Instantiate(targetPrefab, spawnPoint.position, spawnPoint.rotation);
        Vector3 scale = Vector3.one * 1.5f;

        // 右側のキャラだけ左右反転（見た目調整）
        if (!isPlayer1) scale.x *= -1;
        preview.transform.localScale = scale;

        if (isPlayer1)
            leftPreview = preview;
        else
            rightPreview = preview;
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

    // 相手がキャラ変更したら呼ばれる
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("SelectedCharacter"))
        {
            string characterName = changedProps["SelectedCharacter"].ToString();
            ShowCharacterPreview(targetPlayer, characterName);
        }
    }
}
