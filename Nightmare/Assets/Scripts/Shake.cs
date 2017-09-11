using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    public bool active;
    float timeLeft;
    Quaternion rot;

	// Use this for initialization
	void Start () {
        active = false;
        timeLeft = 0.0f;
        rot = gameObject.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                manager.introduceMonster();

                active = false;
                gameObject.transform.rotation = rot;
            }
            gameObject.transform.Rotate(0, 1, 0);
        }
	}

    void OnMouseDown()
    {
        if (active)
        {
            // this object was clicked - do something
            active = false;
            gameObject.transform.rotation = rot;
        }
    }

    public void startShake()
    {
        active = true;
        timeLeft = 4;
    }
}
