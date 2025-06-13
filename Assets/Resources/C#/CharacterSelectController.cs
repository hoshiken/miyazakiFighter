using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CharacterSelectController : MonoBehaviourPunCallbacks
{
    public string characterName = "Sameshima";
    public CharacterPreviewManager previewManager;

    void Start()
{
    // 自分のキャラ設定（＝初期値 "Sameshima" を送信）
    SetSelectedCharacter(characterName);

    // 他プレイヤーがすでにいるならその表示も行う
    foreach (var p in PhotonNetwork.PlayerListOthers)
    {
        if (p.CustomProperties.TryGetValue("SelectedCharacter", out object charName))
        {
            previewManager.ShowCharacterPreview(p, charName.ToString());
        }
        else
        {
            // まだプロパティ未設定なら仮でSameshima表示
            previewManager.ShowCharacterPreview(p, "Sameshima");
        }
    }
}

    public void OnClickSelectCharacter()
    {
        SetSelectedCharacter(characterName);
    }

    private void SetSelectedCharacter(string name)
    {
        characterName = name;

        var hash = new Hashtable { ["SelectedCharacter"] = name };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        previewManager.ShowCharacterPreview(PhotonNetwork.LocalPlayer, name);
        Debug.Log($"選択キャラ: {name}");
    }

    // プレイヤーが部屋に入ったとき
    public override void OnPlayerEnteredRoom(Player newPlayer)
{
    Debug.Log($"Player entered: {newPlayer.NickName}");

    // 2Pが入ってきた直後、1Pから2Pのキャラ表示を試みる
    if (newPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object selectedChar))
    {
        previewManager.ShowCharacterPreview(newPlayer, selectedChar.ToString());
    }
    else
    {
        // 万が一まだプロパティが設定されていない場合、初期値として"Sameshima"を設定しておく
        var hash = new ExitGames.Client.Photon.Hashtable { ["SelectedCharacter"] = "Sameshima" };
        newPlayer.SetCustomProperties(hash); // これ自体は他人には効かないが、デバッグ用
    }
}

    // プレイヤーのCustomPropertiesが更新されたとき
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("SelectedCharacter"))
        {
            string characterName = changedProps["SelectedCharacter"].ToString();
            previewManager.ShowCharacterPreview(targetPlayer, characterName);
        }
    }
}
