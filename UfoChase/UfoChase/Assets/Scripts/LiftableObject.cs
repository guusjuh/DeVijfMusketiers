using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an object that can be lifted by the UFO's beam
public class LiftableObject : MonoBehaviour {
    [SerializeField]
    private float flySpeed;
    private bool flying;

    Rigidbody myBody;

    public void Initialize()
    {
        // startoff on the ground, gravity enabled
        flying = false;

        myBody = GetComponent<Rigidbody>();
    }

    public void Loop()
    {
        if (flying)
        {
            if(transform.position.y >= 20.0f)
            {
                return;
            }
            else
            {
                transform.position += new Vector3(0.0f, flySpeed * Time.deltaTime, 0.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // in beam and not flying already
        if(other.tag == "Spotlight" && !flying)
        {
            flying = true;
            myBody.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // out of beam and flying 
        if (other.tag == "Spotlight" && flying)
        {
            flying = false;
            myBody.useGravity = true;
        }
    }
}
