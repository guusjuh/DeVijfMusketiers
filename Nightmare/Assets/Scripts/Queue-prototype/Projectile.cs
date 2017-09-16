using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed = 2.0f;
    private float lifetime = 100.0f;
	// Update is called once per frame
	void Update () {
		transform.Translate(0.0f, 0.0f, speed, Space.Self);
	    lifetime -= Time.deltaTime;
	    if (lifetime <= 0.0f)
	    {
	        Destroy(gameObject);
	    }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vase")
        {
            //break Vase
        } else if (other.tag == "Human")
        {
            //kill Human
        }
    }
}
