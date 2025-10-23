using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviourPun
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public string trigger;
        public bool requireSneak;
        public bool requireNoDirection;
        public bool requireHorizontal;
    }

    [Header("攻撃パターン設定（追加可能）")]
    public List<AttackPattern> attackPatterns = new List<AttackPattern>();

    [Header("コンボ設定")]
    public float comboResetTime = 0.6f; // コンボ受付時間
    private int comboStep = 0;
    private float lastAttackTime = 0f;

    private playerMove move;
    private Animator anim;
    private bool canCombo = false;

    void Start()
    {
        move = GetComponent<playerMove>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (!move.IsGrounded()) return;

        // 攻撃入力
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryAttack();
        }

        // コンボリセット
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            canCombo = false;
        }
    }

    void TryAttack()
    {
        move.isAttacking = true;
        lastAttackTime = Time.time;

        bool sneaking = move.IsSneaking();
        float horizontal = Input.GetAxisRaw("Horizontal");

        string chosenTrigger = null;

        // 条件に合う攻撃を探す
        foreach (var atk in attackPatterns)
        {
            if (atk.requireSneak && !sneaking) continue;
            if (atk.requireNoDirection && (horizontal != 0)) continue;
            if (atk.requireHorizontal && horizontal == 0) continue;

            chosenTrigger = atk.trigger;
            break;
        }

        // --- コンボ対応 ---
        if (canCombo && chosenTrigger != null)
        {
            chosenTrigger += (comboStep + 1); // 例: punch1 → punch2
            comboStep++;
        }
        else
        {
            comboStep = 0;
        }

        canCombo = true;

        if (chosenTrigger != null)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, chosenTrigger);
        }
    }

    // --- アニメーションイベント用 ---
    public void EndAttack()
    {
        move.isAttacking = false;
    }

    public void EnableCombo()
    {
        canCombo = true;
    }

    public void DisableCombo()
    {
        canCombo = false;
    }
}
