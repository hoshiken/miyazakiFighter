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
        // 自分のキャラを反映
        SetSelectedCharacter(characterName);

        // 相手が既に入っているならそのキャラも表示
        foreach (var p in PhotonNetwork.PlayerListOthers)
        {
            if (p.CustomProperties.TryGetValue("SelectedCharacter", out object charName))
            {
                previewManager.ShowCharacterPreview(p, charName.ToString());
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

    // 相手が参加してきたら表示
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 新しく入ってきたプレイヤーのプロパティを確認
        if (newPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object selectedChar))
        {
            previewManager.ShowCharacterPreview(newPlayer, selectedChar.ToString());
        }
    }
}
