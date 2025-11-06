using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviourPun
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;               // パターン名（例: SneakKick）
        public List<string> triggers;     // 例: ["kick1", "kick2"]
        public bool requireSneak;
        public bool requireNoDirection;
        public bool requireHorizontal;
    }

    [Header("攻撃パターン設定")]
    public List<AttackPattern> attackPatterns = new List<AttackPattern>();

    [Header("コンボ設定")]
    public float comboResetTime = 0.6f;

    [Header("突進設定")]
    public float dashBoostSpeed = 8f;      // 通常速度より速い突進速度
    public float dashDuration = 0.15f;     // 速度上昇の持続時間（秒）

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private AttackPattern currentPattern = null;

    private playerMove move;
    private Animator anim;
    private bool comboWindowOpen = false;
    private bool isDashing = false;        // 突進中フラグ

    void Start()
    {
        move = GetComponent<playerMove>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (!move.IsGrounded()) return;

        // --- 攻撃入力（Kキー） ---
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryAttack();
        }

        // --- 別攻撃入力（Oキーで「bird」） ---
        if (Input.GetKeyDown(KeyCode.O))
        {
            TrySpecialAttack("bird");
        }

        // --- コンボリセット ---
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    // ====================================
    // 🔸通常コンボ攻撃処理
    // ====================================
    void TryAttack()
    {
        if (move.isAttacking && !comboWindowOpen) return;

        bool sneaking = move.IsSneaking();
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (sneaking)
        {
            // しゃがみ中攻撃
            AttackPattern sneakPattern = attackPatterns.Find(a => a.requireSneak);
            if (sneakPattern != null)
                currentPattern = sneakPattern;
            else
            {
                Debug.LogWarning("しゃがみ攻撃パターンが見つかりません！");
                return;
            }
        }
        else
        {
            // 通常攻撃
            if (currentPattern == null)
            {
                foreach (var atk in attackPatterns)
                {
                    if (atk.requireSneak) continue;
                    if (atk.requireNoDirection && (horizontal != 0)) continue;
                    if (atk.requireHorizontal && horizontal == 0) continue;

                    currentPattern = atk;
                    comboStep = 0;
                    break;
                }
            }
        }

        if (currentPattern == null) return;

        // --- しゃがみ維持チェック ---
        if (currentPattern.requireSneak && !sneaking)
        {
            ResetCombo();
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "idle");
            return;
        }

        // --- 攻撃実行 ---
        move.isAttacking = true;
        lastAttackTime = Time.time;

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

    // ====================================
    // 🔸Oキー専用攻撃（bird）
    // ====================================
    void TrySpecialAttack(string triggerName)
    {
        if (move.isAttacking) return;

        move.isAttacking = true;
        lastAttackTime = Time.time;
        photonView.RPC("TriggerAnimRPC", RpcTarget.All, triggerName);
    }

    // ====================================
    // 🔸アニメーショントリガー共通RPC
    // ====================================
    [PunRPC]
    void TriggerAnimRPC(string trigger)
    {
        anim.SetTrigger(trigger);

        // 他のトリガーをリセット
        foreach (var atk in attackPatterns)
        {
            foreach (var t in atk.triggers)
            {
                if (t != trigger) anim.ResetTrigger(t);
            }
        }

        // 🔹「dash」など特定の攻撃で突進を開始する場合（例: kick1など）
        if (trigger.ToLower().Contains("kick") || trigger.ToLower().Contains("dash"))
        {
            StartCoroutine(DashBoostCoroutine());
        }
    }

    // ====================================
    // 🔸突進中だけ速度を上げる処理
    // ====================================
    System.Collections.IEnumerator DashBoostCoroutine()
    {
        if (isDashing) yield break;
        isDashing = true;

        float elapsed = 0f;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float direction = move.IsFacingRight ? 1f : -1f;
            rb.velocity = new Vector2(direction * dashBoostSpeed, rb.velocity.y);
            yield return null;
        }

        isDashing = false;
    }

    // ====================================
    // 🔸コンボ制御など
    // ====================================
    public void OpenComboWindow() => comboWindowOpen = true;
    public void CloseComboWindow() => comboWindowOpen = false;

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
