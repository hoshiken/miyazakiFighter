using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterPreviewManager : MonoBehaviourPunCallbacks
{
    public Transform leftSpawnPoint;   // 1P（Master）表示位置
    public Transform rightSpawnPoint;  // 2P（非Master）表示位置
    public GameObject[] characterPrefabs;

    private GameObject currentPreview;

    private string defaultCharacter = "Sameshima";

    void Start()
    {
        // 初期選択キャラを設定
        SetCharacter(defaultCharacter);
    }

    public void SetCharacter(string characterName)
    {
        // カスタムプロパティにキャラを登録
        var hash = new Hashtable { ["SelectedCharacter"] = characterName };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        // 表示位置を判断（1P: 左, 2P: 右）
        bool isPlayer1 = PhotonNetwork.IsMasterClient;
        Transform spawnPoint = isPlayer1 ? leftSpawnPoint : rightSpawnPoint;

        // 既存の表示を削除
        if (currentPreview != null)
            Destroy(currentPreview);

        // プレハブを取得・生成
        GameObject prefab = GetCharacterPrefab(characterName);
        if (prefab != null)
        {
            currentPreview = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            currentPreview.transform.localScale = Vector3.one * 1.5f; // サイズ調整
        }
        else
        {
            Debug.LogWarning($"キャラ {characterName} のプレハブが見つかりません");
        }
    }

    GameObject GetCharacterPrefab(string name)
    {
        foreach (var prefab in characterPrefabs)
        {
            if (prefab.name == name)
                return prefab;
        }
        return null;
    }

    // キャラ変更時に外部から呼び出す関数
    public void OnClickSelectCharacter(string characterName)
    {
        SetCharacter(characterName);
    }
}
