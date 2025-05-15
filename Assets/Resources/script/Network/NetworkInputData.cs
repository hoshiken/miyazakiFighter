using Fusion;
using UnityEngine;


// 入力構造体（Fusionで使う）
public struct NetworkInputData : INetworkInput
{
    public Vector2 moveDirection;
    public NetworkButtons buttons;
}
