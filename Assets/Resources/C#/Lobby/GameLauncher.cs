using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using Fusion.Photon.Realtime;
using UnityEngine;
using TMPro;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef playerAvatarPrefab;
    [SerializeField] private TextMeshProUGUI playerCountText;
    private NetworkRunner networkRunner;
    private const int MAX_PLAYERS = 20;

    private async void Start()
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.AddCallbacks(this);

        // ✅ Photon Cloud Transport設定
        var transport = networkRunner.gameObject.AddComponent<PhotonRealtimeTransport>();
        transport.AppSettings = Resources.Load<PhotonAppSettings>("PhotonAppSettings");
        transport.Protocol = ExitGames.Client.Photon.ConnectionProtocol.WebSocketSecure;

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "MainLobby",
            SceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        Debug.Log(result);
        UpdatePlayerCount();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined: {player}");
        UpdatePlayerCount();
        if (player == runner.LocalPlayer)
            runner.Spawn(playerAvatarPrefab, Vector2.zero, Quaternion.identity);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {player}");
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        if (playerCountText == null || networkRunner == null) return;
        int count = networkRunner.ActivePlayers.Count();
        playerCountText.text = $"{count}人/{MAX_PLAYERS}人";
    }

    // ===== 空実装 (Fusion 2.0.8対応 全部必須) =====
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
