using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motionTest : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        bool isTriggered = false;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            anim.SetBool("Isidle",false);
            anim.SetTrigger("walk");
            isTriggered = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("sneak");
            isTriggered = true;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            anim.SetTrigger("standUp");
            isTriggered = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetTrigger("jump");
            isTriggered = true;
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            anim.SetBool("IsIdle",false);
            anim.SetTrigger("guard");
            isTriggered = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetTrigger("punch");
            isTriggered = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            anim.SetTrigger("kick");
            isTriggered = true;
        }

        // どのキーも押されていないときだけ idle
        if (!isTriggered)
        {
            anim.SetBool("Isidle",true);
        }
    }
}
