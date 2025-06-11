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
        // 初期キャラを設定
        SetSelectedCharacter(characterName);
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
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("SelectedCharacter", out object myChar))
        {
            previewManager.ShowCharacterPreview(PhotonNetwork.LocalPlayer, myChar.ToString());
        }
    }
}
