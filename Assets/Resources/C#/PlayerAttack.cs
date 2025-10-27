using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviourPun
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;               // パターン名（例: Neutral, Sneak, Side）
        public List<string> triggers;     // 例: ["punch1", "punch2", "punch3"]
        public bool requireSneak;
        public bool requireNoDirection;
        public bool requireHorizontal;
    }

    [Header("攻撃パターン設定（追加しやすい構造）")]
    public List<AttackPattern> attackPatterns = new List<AttackPattern>();

    [Header("コンボ設定")]
    public float comboResetTime = 0.6f;
    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private AttackPattern currentPattern = null;

    private playerMove move;
    private Animator anim;
    private bool comboWindowOpen = false;

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
            ResetCombo();
        }
    }

    void TryAttack()
    {
        if (move.isAttacking && !comboWindowOpen) return;

        move.isAttacking = true;
        lastAttackTime = Time.time;

        bool sneaking = move.IsSneaking();
        float horizontal = Input.GetAxisRaw("Horizontal");

        // --- 条件に合う攻撃パターンを検索 ---
        if (currentPattern == null)
        {
            // しゃがみ中はしゃがみ攻撃を優先して探す
            List<AttackPattern> patterns = new List<AttackPattern>(attackPatterns);
            if (sneaking)
            {
                // しゃがみ攻撃だけに絞り込む
                patterns = attackPatterns.FindAll(a => a.requireSneak);
            }

            foreach (var atk in patterns)
            {
                if (atk.requireSneak && !sneaking) continue;

                // しゃがみ攻撃では方向入力の有無は無視
                if (!atk.requireSneak)
                {
                    if (atk.requireNoDirection && (horizontal != 0)) continue;
                    if (atk.requireHorizontal && horizontal == 0) continue;
                }

                currentPattern = atk;
                comboStep = 0;
                break;
            }
        }

        if (currentPattern == null) return;

        // --- コンボ発動 ---
        if (comboStep < currentPattern.triggers.Count)
        {
            string trigger = currentPattern.triggers[comboStep];
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, trigger);
            comboStep++;
            comboWindowOpen = false;
        }
        else
        {
            ResetCombo();
        }
    }


    public void OpenComboWindow()
    {
        comboWindowOpen = true;
    }

    public void CloseComboWindow()
    {
        comboWindowOpen = false;
    }

    public void EndAttack()
    {
        move.isAttacking = false;
        photonView.RPC("TriggerAnimRPC", RpcTarget.All, "idle");
        ResetCombo();
    }

    void ResetCombo()
    {
        comboStep = 0;
        currentPattern = null;
        comboWindowOpen = false;
    }
}
