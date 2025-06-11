using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class CharacterSelectController : MonoBehaviourPunCallbacks
{
    public string characterName = "Sameshima";  // 初期値を設定
    public CharacterPreviewManager previewManager;

    public void Start()
    {
        // ゲーム開始時に自動で"Sameshima"を選択
        var hash = new Hashtable { ["SelectedCharacter"] = characterName };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log($"Default selected character: {characterName}");
    }

    public void OnClickSelectCharacter()
    {
        var hash = new Hashtable { ["SelectedCharacter"] = characterName };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log($"Selected character: {characterName}");
    }
    public void OnClick()
    {
        previewManager.ShowCharacterPreview(characterName);
    }
}
