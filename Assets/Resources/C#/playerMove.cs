using UnityEngine;
using UnityEngine.U2D.IK;
using Photon.Pun;

public class playerMove : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Vector3 startPosition = new Vector3(0f, 1f, 0f);
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGrounded = true;

    private float horizontalInput = 0f;
    private bool jumpPressed = false;

    private Vector3 networkScale;
    private string currentTrigger = "";

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

    // 不要な有効無効の切り替えは削除
    if (ikManager != null) ikManager.weight = 1f;

    transform.position = startPosition;
    rb.simulated = true;
}

[PunRPC]
void TriggerAnimRPC(string trigger)
{
    if (anim != null)
    {
        anim.SetTrigger(trigger);
    }
    else
    {
        Debug.LogWarning($"Animator is null when trying to set trigger: {trigger}");
    }
}


    void Update()
    {
        if (!photonView.IsMine) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

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
        else
        {
            if (isGrounded) photonView.RPC("TriggerAnimRPC", RpcTarget.All, "idle");
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "jump");
            jumpPressed = true;
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "sneak");
        }
        else if (Input.GetKeyUp(KeyCode.S) && isGrounded)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "standUp");
        }
        if (Input.GetKeyDown(KeyCode.P) && isGrounded)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "punch");
        }
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            photonView.RPC("TriggerAnimRPC", RpcTarget.All, "kick");
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 velocity = rb.velocity;
        velocity.x = horizontalInput * moveSpeed;

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

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    [PunRPC]
    void SetDirectionRPC(float xScale)
    {
        transform.localScale = new Vector3(xScale, 1, 1);
    }

    // 予備：同期が必要な場合に備えてスケール補完などを追加したいとき
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 今回はRPCだけで同期するので何も書かなくてもOK
    }
}
