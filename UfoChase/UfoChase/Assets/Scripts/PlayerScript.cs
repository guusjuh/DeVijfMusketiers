using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerScript : MonoBehaviour {
    // movement variables
    [SerializeField]
    private float moveSpeed;
    private float moveInput = 0;
    [SerializeField]
    private float turnSpeed;
    private float turnInput = 0;
    private float prevMousePos = 0;
    private float currMousePos = 0;

    // if mouse clicked once and hold: move and rotation 
    // if mouse clicked twice and hold: rotation
    private bool lookingAround = false;
    private bool clicked = false;
    private static float TOTAL_CLICK_COOLDOWN = 0.5f;
    private float clickCooldown = 0;

    private bool inSpot;
    private bool inHiding;

    // properties
    public bool CanMove {
        get
        {
            return (!inSpot && !lookingAround);
        }
    } 

    public void Initialize(Vector3 spawnPos)
    {
        transform.position = spawnPos;

        currMousePos = Input.mousePosition.x;
        prevMousePos = currMousePos;

        clickCooldown = TOTAL_CLICK_COOLDOWN;

        inSpot = false;
    }

    public void Loop()
    {
        // player is catched by the UFO! 
        if (inSpot)
        {
            Debug.Log("You dead!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("gameover");
        }

        // catch current mouse position
        currMousePos = Input.mousePosition.x;

        // reduce time to check for double click
        if (clicked)
        {
            clickCooldown -= Time.deltaTime;
            if(clickCooldown <= 0)
            {
                clickCooldown = 0;
                clicked = false;
            }
        }

        // obtain input for turning and moving
        GetInput();

        prevMousePos = currMousePos;

        // process input
        // move the player
        Debug.Log("moving is " + (moveInput == 0 ? "false" : "true"));

        transform.position += transform.forward * (moveInput * moveSpeed);

        transform.Rotate(new Vector3(0, 1, 0), turnSpeed * turnInput);
    }

    private void GetInput()
    {
        // if pressed, check for double click 
        if(Input.GetMouseButtonDown(0))
        {
            if (clicked)
            {
                lookingAround = true;
            }
            else
            {
                lookingAround = false;
                clicked = true;
                clickCooldown = TOTAL_CLICK_COOLDOWN;
            }
        }

        // no double click = looking around and moving
        if (!lookingAround)
        {
            if (CanMove)
            {
                // is the mouse pressed?
                if (Input.GetMouseButton(0))
                {
                    // set movement input
                    // TODO: gradually increase value over time
                    moveInput = 1;

                    // set turn input
                    float deltaMousePos = currMousePos - prevMousePos;
                    turnInput = deltaMousePos;
                }
                else
                {
                    moveInput = 0;
                    turnInput = 0;
                }
            }
        }
        // double click = only looking around
        else
        {
            Debug.Log("looking around");
            // is the mouse pressed?
            if (Input.GetMouseButton(0))
            {
                // set turn input
                float deltaMousePos = currMousePos - prevMousePos;
                turnInput = deltaMousePos;
                moveInput = 0;
            }
            else
            {
                moveInput = 0;
                turnInput = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hiding")
        {
            inHiding = true;
        }

        if (other.tag == "Spotlight" && !inHiding)
        {
            inSpot = true;
            moveInput = 0;
            turnInput = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hiding")
        {
            inHiding = false;
        }
    }
}
