using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; // 移動速度
    [SerializeField] float jumpHight = 1f; // 移動速度
    private Animator anim = null;
    private Rigidbody2D rb = null;

    void Start()
    {
        anim = GetComponent<Animator>(); // Animatorコンポーネントを取得
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得
    }

    void Update()
    {
        //キー入力されたら行動する
        float horizontalKey = Input.GetAxis("Horizontal");
        float verticalKey = Input.GetAxis("Vertical");
        float xSpeed = 0.0f; 
        float ySpeed = 0f;

        if (horizontalKey > 0)
        {
            
            anim.SetBool("IsIdle",false);
            transform.localScale = new Vector3(1, 1, 1);
            anim.SetTrigger("walk");
            xSpeed = moveSpeed;
        }
        else if (horizontalKey < 0)
        {
            anim.SetBool("IsIdle",false);
            transform.localScale = new Vector3(-1, 1, 1);
            anim.SetTrigger("walk");
            xSpeed = -moveSpeed;
        }
        else if (verticalKey > 0)
        {
            anim.SetBool("IsIdle",false);
            anim.SetTrigger("jump");
            ySpeed = jumpHight;
        }
        else
        {
            anim.SetBool("IsIdle",true);
            xSpeed = 0.0f;
        }
        
        rb.velocity = new Vector2(xSpeed, rb.velocity.y);
    }
}
