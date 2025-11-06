using UnityEngine;
using Photon.Pun;

public class PlayerDamageReceiver : MonoBehaviourPun
{
    private Animator anim;
    private playerMove move;

    [Header("アニメーション名設定")]
    public string normalHitAnim = "hit";
    public string finisherHitAnim = "blow";
    public string specialHitAnim = "specialHit";
    public string guardStandAnim = "guard";
    public string guardCrouchAnim = "guardLow";

    void Start()
    {
        anim = GetComponent<Animator>();
        move = GetComponent<playerMove>();
    }

    public void ReceiveAttack(int damage, AttackType type, bool isGuardArea, bool isCrouching)
    {
        Debug.Log($"[Receiver] ReceiveAttack called: damage={damage}, guard={isGuardArea}, crouch={isCrouching}");

        if (isGuardArea)
        {
            photonView.RPC("PlayGuardAnimRPC", RpcTarget.All, isCrouching);
            Debug.Log("guarded");
        }
        else
        {
            photonView.RPC("PlayHitAnimRPC", RpcTarget.All, (int)type);
            Debug.Log("ouch!");
        }
    }

    [PunRPC]
    void PlayHitAnimRPC(int typeInt)
    {
        AttackType type = (AttackType)typeInt;
        string trigger = normalHitAnim;

        switch (type)
        {
            case AttackType.ComboFinisher: trigger = finisherHitAnim; break;
            case AttackType.Special: trigger = specialHitAnim; break;
        }

        anim.SetTrigger(trigger);
    }

    [PunRPC]
    void PlayGuardAnimRPC(bool crouch)
    {
        string trigger = crouch ? guardCrouchAnim : guardStandAnim;
        anim.SetTrigger(trigger);
    }
}
