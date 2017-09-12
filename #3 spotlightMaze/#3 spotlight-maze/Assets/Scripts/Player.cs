using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    bool dead;
    bool won;
    [SerializeField]
    private float speed = 250.0f;
    [SerializeField]
    private float jumpHeight = 10.0f;
    private Vector3 start;
    private Rigidbody rb;
    Accelaratable ac;
    Vector2 velocity;
    public GameObject winMsg;
    public GameObject loseMsg;
    public GameObject spotMsg;
    bool mobile = false;

    void Start()
    {
        start = transform.position;
        dead = false;
        won = false;
        ac = new Accelaratable();
        velocity = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        if (Application.platform == RuntimePlatform.Android)
        {
            mobile = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
    
    // Update is called once per frame
	void Update () {
        if (!dead)
        {
            if (ac != null)
            {
                if (mobile)
                {
                    ac.Update();
                }
                else
                {
                    ac.Update2();
                }
                
                velocity = ac.Acceleration * speed;
                if (rb != null)
                {
                    rb.AddForce(velocity.x, velocity.y, 0.0f);
                }
            }
            else
            {
                Debug.Log("No Acceleration dude :(");
            }
        }
	}

    private IEnumerator ResetInSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        Reset();
    }

    private IEnumerator JumpTimer(float sec)
    {
        float half = sec / 2.0f;
        while(sec > half)
        {
            rb.AddForce(0.0f, 0.0f, -jumpHeight);
            Debug.Log(1);
            //transform.Translate(new Vector3(0.0f, 0.0f, -0.2f));
            sec -= Time.deltaTime;
            yield return null;
        }
        while (sec > 0.0f)
        {
            rb.AddForce(0.0f, 0.0f, jumpHeight);
            Debug.Log(2);
            //transform.Translate(new Vector3(0.0f, 0.0f, 0.2f));
            sec -= Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, start.z);
    }

    private void Reset()
    {
        dead = false;
        won = false;
        loseMsg.SetActive(false);
        winMsg.SetActive(false);
        spotMsg.SetActive(false);
        transform.position = start;
        GetComponent<MeshRenderer>().enabled = true;

    }

    public void Die()
    {
        if (!won)
        {
            loseMsg.SetActive(true);
            dead = true;
        }

        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(ResetInSeconds(1.0f));
    }

    public void Spotted()
    {
        if (!won || !dead)
        {
            spotMsg.SetActive(true);
            dead = true;
        }

        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(ResetInSeconds(1.0f));
    }

    public void Win()
    {
        won = true;
        winMsg.SetActive(true);
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(ResetInSeconds(1.0f));
    }

    public void Jump()
    {
        Debug.Log("Boing");
        StartCoroutine(JumpTimer(0.5f));
    }
}
