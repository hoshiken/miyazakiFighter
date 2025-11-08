using Mirror;
using TMPro;
using UnityEngine;

public class WebGLClientUI : MonoBehaviour
{
    [SerializeField] TMP_InputField addressInput;   // 任意: 入力欄
    [SerializeField] TMP_Text statusText;           // 任意: 状態表示

    MyRoomManager Mgr => (MyRoomManager)NetworkManager.singleton;

    public void Connect()
    {
        if (addressInput && !string.IsNullOrEmpty(addressInput.text))
            Mgr.networkAddress = addressInput.text.Trim();

        statusText?.SetText($"connecting {Mgr.networkAddress}...");
        Mgr.StartClient();
    }
    public void Disconnect()
    {
        Mgr.StopClient();
        statusText?.SetText("disconnected");
    }
}
