using Fusion;
using UnityEngine;
 using System;
 using System.Collections.Generic;
 using Fusion.Sockets;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
 {
     [SerializeField]private NetworkRunner networkRunnerPrefab;
     [SerializeField]private NetworkPrefabRef playerAvatarPrefab;
     [SerializeField]private GameObject spawnObj;

     private async void Start() {
         var networkRunner = Instantiate(networkRunnerPrefab);
         // GameLauncherを、NetworkRunnerのコールバック対象に追加する
         networkRunner.AddCallbacks(this);
         var result = await networkRunner.StartGame(new StartGameArgs {
             GameMode = GameMode.Shared
         });
     }

     void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
     void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
     // プレイヤーがセッションへ参加した時に呼ばれるコールバック
     void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
     {
         Vector2 spawnPosition = spawnObj.transform.position;
         // セッションへ参加したプレイヤーが自分自身かどうかを判定する
        if (player == runner.LocalPlayer) {
             // 自分自身のアバターをスポーンする
             runner.Spawn(playerAvatarPrefab, spawnPosition, Quaternion.identity);
         }
     }
     void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
     void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) {}
     void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
     void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
     void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) {}
     void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
     void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
     void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
     void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
     void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
     void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
     void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
     void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
     void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
     void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) {}
     void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) {}
 }