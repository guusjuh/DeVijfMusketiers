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
    private bool moving = false;
    private bool clicked = false;
    private static float TOTAL_CLICK_COOLDOWN = 0.5f;
    private float clickCooldown = 0;

    private bool inSpot;
    public bool InSpot { get { return inSpot; } }
    private bool inHiding;
    private bool finished;
    public bool Finished { get { return finished; } }

    // properties
    public bool CanMove {
        get
        {
            return (!inSpot && moving && !finished);
        }
    } 

    public void Initialize(Vector3 spawnPos)
    {
        transform.position = new Vector3(spawnPos.x, transform.position.y, spawnPos.z);

        currMousePos = Input.mousePosition.x;
        prevMousePos = currMousePos;

        clickCooldown = TOTAL_CLICK_COOLDOWN;

        inSpot = false;
        finished = false;
        inHiding = false;
    }
     
    public void Loop()
    {
        // catch current mouse position
        /*if(Input.touchCount > 0)
        {
            Touch zeroTouch = Input.GetTouch(0);
            currMousePos = zeroTouch.position.x; //Input.mousePosition.x;
        }*/
        // reduce time to check for double click
        if (clicked)
        {
            clickCooldown -= Time.deltaTime;
            if (clickCooldown <= 0)
            {
                clickCooldown = 0;
                clicked = false;
            }
        }

        // catch current touch position
        if (Input.touchCount > 0)
        {
            Touch startTouch = Input.GetTouch(0);

            switch (startTouch.phase)
            {
                case TouchPhase.Began:
                    currMousePos = startTouch.position.x;
                    prevMousePos = currMousePos;
                    if (clicked) {
                        moving = true;
                    }
                    break;

                case TouchPhase.Moved:
                    currMousePos = startTouch.position.x;
                    break;

                case TouchPhase.Ended:
                    if (!clicked) {
                        moving = false;
                        clicked = true;
                        clickCooldown = TOTAL_CLICK_COOLDOWN;
                    }
                    break;
            }

            GetInput();

            prevMousePos = currMousePos;
        }


        // obtain input for turning and moving
        //GetInput();

        //prevMousePos = currMousePos;

        // process input
        // move the player
        transform.position += transform.forward * (moveInput * moveSpeed);

        transform.Rotate(new Vector3(0, 1, 0), turnSpeed * turnInput);
    }

    private void GetInput()
    {
        // no double click = looking around and moving
        if (moving)
        {
            if (CanMove)
            {
                // is the mouse pressed?
                if (Input.touchCount > 0)//if (Input.GetMouseButton(0))
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
            else
            {
                moveInput = 0;
                turnInput = 0;
            }
        }
        // double click = only looking around
        else
        {
            // is the mouse pressed?
            if (Input.touchCount > 0) //if (Input.GetMouseButton(0))
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

        //-------------------------------------------------
        // if pressed, check for double click 
        /*if(Input.touchCount > 0)//if(Input.GetMouseButtonDown(0))
        {
            if (clicked)
            {
                moving = true;
            }
            else
            {
                moving = false;
                clicked = true;
                clickCooldown = TOTAL_CLICK_COOLDOWN;
            }
        }

        // no double click = looking around and moving
        if (moving)
        {
            if (CanMove)
            {
                // is the mouse pressed?
                if(Input.touchCount > 0)//if (Input.GetMouseButton(0))
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
            // is the mouse pressed?
            if (Input.touchCount > 0) //if (Input.GetMouseButton(0))
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
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "End")
        {
            finished = true;
            moveInput = 0;
            turnInput = 0;

            StartCoroutine(WaitForFinish());
        }

        if (other.tag == "Hiding")
        {
            inHiding = true;
        }

        if (other.tag == "Spotlight" && !inHiding || other.tag == "Box")
        {
            inSpot = true;
            moveInput = 0;
            turnInput = 0;

            StartCoroutine(WaitForDead());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hiding")
        {
            inHiding = false;
        }
    }

    private IEnumerator WaitForDead()
    {
        yield return new WaitForSeconds(1.0f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("gameover");
    }

    private IEnumerator WaitForFinish()
    {
        yield return new WaitForSeconds(1.0f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("win");
    }
}
