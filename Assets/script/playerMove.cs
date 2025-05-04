using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class playerMove : MonoBehaviour
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
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // アニメーション制御
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

        // ジャンプ入力
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            anim.SetTrigger("jump");
            jumpPressed = true;
            isGrounded = false;
        }

        // しゃがみ
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
        Vector2 velocity = rb.velocity;

        // 横移動
        velocity.x = horizontalInput * moveSpeed;
        //Debug.Log("aaaa");

        // ジャンプ処理
        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = false;
        }
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
