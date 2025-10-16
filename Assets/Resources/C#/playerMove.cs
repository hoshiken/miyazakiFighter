using UnityEngine;
using UnityEngine.U2D.IK;
using Photon.Pun;

public class playerMove : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGrounded = true;

    private float horizontalInput = 0f;
    private bool jumpPressed = false;
    private bool isSneaking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        var ikManager = GetComponent<IKManager2D>();

        if (!photonView.IsMine)
        {
            rb.simulated = false;
            return;
        }

        if (ikManager != null) ikManager.weight = 1f;
        rb.simulated = true;
    }

    [PunRPC]
    void TriggerAnimRPC(string trigger)
    {
        if (anim != null)
        {
            anim.SetTrigger(trigger);
        }
    }

    [PunRPC]
    void SetSneakRPC(bool value)
    {
        if (anim != null)
        {
            anim.SetBool("isSneaking", value);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // --- スニーク制御（しゃがみ優先）---
        if (Input.GetKey(KeyCode.S) && isGrounded)
        {
            if (!isSneaking)
            {
                isSneaking = true;
                photonView.RPC("SetSneakRPC", RpcTarget.AllBuffered, true);
            }
        }
        else
        {
            if (isSneaking)
            {
                isSneaking = false;
                photonView.RPC("SetSneakRPC", RpcTarget.AllBuffered, false);
            }
        }

        // --- 移動処理 ---
        if (!isSneaking)
        {
            if (horizontalInput > 0)
            {
                photonView.RPC("SetDirectionRPC", RpcTarget.AllBuffered, 1f);
                if (isGrounded) photonView.RPC("TriggerAnimRPC", RpcTarget.All, "walk");
            }
            else if (horizontalInput < 0)
            {
                photonView.RPC("SetDirectionRPC", RpcTarget.AllBuffered, -1f);
                if (isGrounded) photonView.RPC("TriggerAnimRPC", RpcTarget.All, "walk");
            }
            else if (isGrounded)
            {
                photonView.RPC("TriggerAnimRPC", RpcTarget.All, "idle");
            }

            // --- ジャンプ処理（しゃがみ中は不可）---
            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
            {
                photonView.RPC("TriggerAnimRPC", RpcTarget.All, "jump");
                jumpPressed = true;
                isGrounded = false;
            }
        }

        // --- 攻撃系 ---
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            if (isSneaking)
                photonView.RPC("TriggerAnimRPC", RpcTarget.All, "kick");
            else
                photonView.RPC("TriggerAnimRPC", RpcTarget.All, "punch");
        }
    }


    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 velocity = rb.velocity;

        // --- スニーク中は移動停止 ---
        if (!isSneaking)
        {
            velocity.x = horizontalInput * moveSpeed;
        }
        else
        {
            velocity.x = 0f;
        }

        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = false;
        }

        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!photonView.IsMine) return;
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    [PunRPC]
    void SetDirectionRPC(float xScale)
    {
        transform.localScale = new Vector3(xScale, 1, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 今回はRPCで同期しているため空
    }
}
