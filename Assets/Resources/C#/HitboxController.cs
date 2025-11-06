using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider2D))]
public class HitboxController : MonoBehaviourPun
{
    public int damage = 10;
    public AttackType attackType = AttackType.Normal;
    public string ownerTag = "Player";

    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    // 攻撃発生フレームでアニメーションイベントから呼ぶ
    public void ActivateHitbox() => col.enabled = true;
    // 攻撃終了フレームで呼ぶ
    public void DeactivateHitbox() => col.enabled = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;
        if (other.CompareTag(ownerTag)) return; // 自分自身には当たらない

        Debug.Log($"[Hitbox] TriggerEnter with {other.name}");

        HurtboxController hurtbox = other.GetComponent<HurtboxController>();
        if (hurtbox != null)
        {
            Debug.Log($"[Hitbox] Hurtbox found on {other.name}, sending RPC...");
            hurtbox.photonView.RPC("OnHitByAttack", RpcTarget.All, damage, (int)attackType);
        }
        else
        {
            Debug.Log($"[Hitbox] No HurtboxController found on {other.name}");
        }
    }
}
