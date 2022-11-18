using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool shiftKeyWasPressed;
    public float moveSpeed = 5f;
    public Rigidbody2D rigidBody;
    Vector2 movement;
    public static Player instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !shiftKeyWasPressed)
        {
            shiftKeyWasPressed = true;
            moveSpeed = 8f;
            Debug.Log("is running");
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && shiftKeyWasPressed)
        {
            shiftKeyWasPressed = false;
            moveSpeed = 5f;
            Debug.Log("stopped running");
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");             
    }

    void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + moveSpeed * Time.fixedDeltaTime * movement);
    }
}
