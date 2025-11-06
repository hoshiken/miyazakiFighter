using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : NetworkBehaviour
{
    public void OnClickSelectCharacter(string characterName)
    {
        CmdSelectCharacter(characterName);
    }

    [Command]
    void CmdSelectCharacter(string name, NetworkConnectionToClient sender = null)
    {
        Debug.Log($"{sender.connectionId} が {name} を選択");

        // キャラ情報を保存して対戦画面へ移行
        TargetGoToBattleScene(sender, name);
    }

    [TargetRpc]
    void TargetGoToBattleScene(NetworkConnection target, string name)
    {
        // 選択情報をGameManagerなどに渡して
        PlayerPrefs.SetString("SelectedCharacter", name);
        SceneManager.LoadScene("BattleScene");
    }
}
