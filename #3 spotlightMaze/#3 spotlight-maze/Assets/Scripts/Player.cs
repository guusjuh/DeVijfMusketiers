using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    bool dead;
    private float speed;
    private Vector3 start;
    Accelaratable ac;
    Vector2 velocity;

    void Start()
    {
        start = transform.position;
        dead = false;
        ac = new Accelaratable();
        velocity = Vector3.zero;
        speed = 0.2f;
    }
    
    // Update is called once per frame
	void Update () {
        if (!dead)
        {
            if (ac != null)
            {
                ac.Update();
                velocity = ac.Acceleration * speed;
                transform.Translate(velocity.x, velocity.y, 0.0f);
            }
            else
            {
                Debug.Log("No Acceleration dude :(");
            }
        }
	}

    private IEnumerator DeathTimer(float sec)
    {
        yield return new WaitForSeconds(sec);
        Reset();
    }

    private IEnumerator JumpTimer(float sec)
    {
        float half = sec / 2.0f;
        while(sec > half)
        {
            transform.Translate(new Vector3(0.0f, 0.0f, -0.2f));
            sec -= Time.deltaTime;
            yield return null;
        }
        while (sec > 0.0f)
        {
            transform.Translate(new Vector3(0.0f, 0.0f, 0.2f));
            sec -= Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, start.z);
    }

    private void Reset()
    {
        dead = false;
        transform.position = start;
        GetComponent<MeshRenderer>().enabled = true;

    }

    public void Die()
    {
        dead = true;
        Debug.Log("U suck D:");
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(DeathTimer(1.0f));
    }

    public void Jump()
    {
        Debug.Log("Boing");
        StartCoroutine(JumpTimer(0.25f));
    }
}
