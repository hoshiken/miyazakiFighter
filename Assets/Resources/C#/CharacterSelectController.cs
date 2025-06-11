using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class CharacterSelectController : MonoBehaviourPunCallbacks
{
    public string characterName;

    public void OnClickSelectCharacter()
    {
        var hash = new Hashtable { ["SelectedCharacter"] = characterName };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log($"Selected character: {characterName}");
    }
}
