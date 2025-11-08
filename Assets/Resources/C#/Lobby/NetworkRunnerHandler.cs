using System;
using System.Linq;                  // Count() 用
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using TMPro;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;
    public TextMeshProUGUI playerCountText;

    private const int MAX_PLAYERS = 20;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        Debug.Log("Starting Fusion session...");

        var result = await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "MainLobby",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
            Debug.Log("✅ Fusion session started successfully!");
        else
            Debug.LogError($"❌ StartGame failed: {result.ShutdownReason}");
    }

    // ======== Fusion コールバック ========
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) => UpdatePlayerCount();
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)   => UpdatePlayerCount();

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    {
        // ここでUIに「切断」などを表示してもOK
        UpdatePlayerCount();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    // 入力は今回は未使用（ロビー表示だけなので空でOK）
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    // AOI（Area Of Interest）イベント（必須になったため空実装）
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    // Reliable送受信（必須シグネチャ）
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    // シーンロード通知（必須シグネチャ）
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner)  { }

    // ======== 人数表示 ========
    private void UpdatePlayerCount()
    {
        if (playerCountText == null || runner == null) return;
        // ActivePlayers は IEnumerable<PlayerRef>
        int count = runner.ActivePlayers != null ? runner.ActivePlayers.Count() : 0;
        playerCountText.text = $"{count}人 / {MAX_PLAYERS}人";
    }
}
