using System.Collections;
using UnityEngine;
using UnityEngine.U2D.IK;
using Photon.Pun;

public class playerMove : MonoBehaviourPun
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Vector3 startPosition = new Vector3(0f, 1f, 0f);
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private bool isGrounded = true;

    private float horizontalInput = 0f;
    private bool jumpPressed = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        var ikManager = GetComponent<IKManager2D>();

        // 操作対象が自分のプレイヤーでなければ物理演算停止
        if (!photonView.IsMine)
        {
            rb.simulated = false;
            enabled = false;
            return;
        }

        anim.enabled = false;
        rb.simulated = false;
        if (ikManager != null)
        {
            ikManager.weight = 0f;
        }

        transform.position = startPosition;

        anim.enabled = true;
        rb.simulated = true;
        if (ikManager != null)
        {
            ikManager.weight = 1f;
        }
    }

    void Update()
    {
        // 自分のプレイヤーのみ操作
        if (!photonView.IsMine) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            if (isGrounded) anim.SetTrigger("walk");
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            if (isGrounded) anim.SetTrigger("walk");
        }
        else
        {
            if (isGrounded) anim.SetTrigger("idle");
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            anim.SetTrigger("jump");
            jumpPressed = true;
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            anim.SetTrigger("sneak");
        }
        else if (Input.GetKeyUp(KeyCode.S) && isGrounded)
        {
            anim.SetTrigger("standUp");
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
}
