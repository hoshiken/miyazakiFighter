using Fusion;
using UnityEngine;
using System;
using System.Collections.Generic;
using Fusion.Sockets;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef playerAvatarPrefab;
    [SerializeField] private GameObject spawnObj;

    private async void Start()
    {
        var networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.ProvideInput = true;
        networkRunner.AddCallbacks(this);

        var result = await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "TestRoom", // 任意のセッション名
            PlayerCount = 2
        });
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        NetworkObject playerObj;
        if (runner.LocalPlayer == player)
        {
            // 1P（自分）
            Vector3 spawnPos = new Vector3(-7f, -7f, 0f);
            playerObj = runner.Spawn(playerAvatarPrefab, spawnPos, Quaternion.identity, player);
        }
        else
        {
            // 2P（他プレイヤー）
            Vector3 spawnPos = new Vector3(7f, -7f, 0f);
            playerObj = runner.Spawn(playerAvatarPrefab, spawnPos, Quaternion.identity, player);

            // 左右反転
            playerObj.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData
        {
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0)
        };
        input.Set<NetworkInputData>(data); // 必ず型引数 <NetworkInputData> を付ける
    }

    // その他コールバック（空実装）
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
}
