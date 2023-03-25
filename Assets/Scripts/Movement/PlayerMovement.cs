using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 movement;
    private Animator animator;
    private bool shiftKeyWasPressed;
    public float moveSpeed = 5f;
    public Rigidbody2D rigidBody;
    public static PlayerMovement instance;

    private void Start()
    {
        instance = this;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !shiftKeyWasPressed)
        {
            shiftKeyWasPressed = true;
            moveSpeed = 8f;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && shiftKeyWasPressed)
        {
            shiftKeyWasPressed = false;
            moveSpeed = 5f;
        }
        SetAnimationMovement();
        if (movement != Vector2.zero)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate() => rigidBody.MovePosition(rigidBody.position + moveSpeed * Time.fixedDeltaTime * movement);

    private void SetAnimationMovement()
    {
        if (movement.x == 0 && movement.y == 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        if (moveSpeed == 5 && (movement.x != 0 || movement.y != 0))
            animator.SetBool("isWalking", true);
        if (moveSpeed == 8 && (movement.x != 0 || movement.y != 0))
            animator.SetBool("isRunning", true);
        else
            animator.SetBool("isRunning", false);
    }
}
