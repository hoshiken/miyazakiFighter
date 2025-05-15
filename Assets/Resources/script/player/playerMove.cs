using UnityEngine;
using UnityEngine.U2D.IK;
using Fusion;

[RequireComponent(typeof(NetworkRigidbody2D))]
public class PlayerMove : NetworkBehaviour
{
    public enum InputButtons
    {
        Jump = 0,
        Sneak = 1
    }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 startPosition = new Vector3(0f, 1f, 0f);

    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private NetworkRigidbody2D networkRigidbody;

    [Networked] private float horizontalInput { get; set; }
    [Networked] private NetworkBool isGrounded { get; set; }
    [Networked] private NetworkBool jumpPressed { get; set; }
    [Networked] private NetworkBool isSneaking { get; set; }

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (networkRigidbody == null) networkRigidbody = GetComponent<NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        var ikManager = GetComponent<IKManager2D>();

        if (!HasStateAuthority)
        {
            networkRigidbody.Rigidbody.simulated = false;
            enabled = false;
            return;
        }

        anim.enabled = false;
        networkRigidbody.Rigidbody.simulated = false;
        if (ikManager) ikManager.weight = 0f;

        transform.position = startPosition;

        anim.enabled = true;
        networkRigidbody.Rigidbody.simulated = true;
        if (ikManager) ikManager.weight = 1f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        CheckGroundStatus(); // 接地判定を更新
        ProcessInput();
        ApplyMovement();
        UpdateAnimation();
    }

    private void CheckGroundStatus()
    {
        // 地面との接触をRaycastで確認
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        isGrounded = hit.collider != null;
    }

    private void ProcessInput()
    {
        if (GetInput(out NetworkInputData input))
        {
            Debug.Log("Horizontal Input: " + input.moveDirection.x);
            horizontalInput = input.moveDirection.x;

            if (horizontalInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontalInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            // ジャンプ
            if (input.buttons.IsSet((int)InputButtons.Jump) && isGrounded)
            {
                jumpPressed = true;
                isGrounded = false;
            }

            // しゃがみ
            isSneaking = input.buttons.IsSet((int)InputButtons.Sneak);
        }
    }

    private void ApplyMovement()
    {
        var velocity = networkRigidbody.Rigidbody.velocity;
        velocity.x = horizontalInput * moveSpeed;

        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumpPressed = false;
        }

        networkRigidbody.Rigidbody.velocity = velocity;
    }

    private void UpdateAnimation()
{
    if (GetInput(out NetworkInputData input))
    {
        anim.SetBool("IsIdle", false);
        if (input.buttons.IsSet((int)InputButtons.Jump))
        {
            anim.SetTrigger("jump");
        }
        else if (input.buttons.IsSet((int)InputButtons.Sneak))
        {
            anim.SetTrigger("sneak");
        }
        else if (Mathf.Abs(input.moveDirection.x) > 0.1f)
        {
            anim.SetBool("IsWalk",true);
        }
        else
        {
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsWalk",false);
        }
    }
}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!HasStateAuthority) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 接地判定の可視化
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
