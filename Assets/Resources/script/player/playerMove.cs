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

        ProcessInput();
        ApplyMovement();
        UpdateAnimation();
    }

    private void ProcessInput()
    {
        if (GetInput(out NetworkInputData input))
        {
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
        if (!isGrounded)
        {
            anim.SetTrigger("jump");
        }
        else if (isSneaking)
        {
            anim.SetTrigger("sneak");
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            anim.SetTrigger("walk");
        }
        else
        {
            anim.SetTrigger("idle");
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
}

// 入力構造体（Fusionで使う）
public struct NetworkInputData : INetworkInput
{
    public Vector2 moveDirection;
    public NetworkButtons buttons;
}

