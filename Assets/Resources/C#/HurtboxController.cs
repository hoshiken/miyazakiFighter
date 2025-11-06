using UnityEngine;
using Photon.Pun;

public class HurtboxController : MonoBehaviourPun
{
    public bool isGuardArea = false; // trueならガード判定用
    public bool isCrouching = false; // しゃがみ中かどうか（PlayerMoveから反映）

    [PunRPC]
    public void OnHitByAttack(int damage, int attackTypeInt)
    {
        Debug.Log($"[Hurtbox] Received hit: {damage}, type={attackTypeInt}, owner={gameObject.name}");

        AttackType type = (AttackType)attackTypeInt;
        var receiver = GetComponentInParent<PlayerDamageReceiver>();
        if (receiver != null)
        {
            Debug.Log("[Hurtbox] Found PlayerDamageReceiver, passing attack info");
            receiver.ReceiveAttack(damage, type, isGuardArea, isCrouching);
        }
        else
        {
            Debug.LogWarning("[Hurtbox] PlayerDamageReceiver not found in parent!");
        }
    }
}
