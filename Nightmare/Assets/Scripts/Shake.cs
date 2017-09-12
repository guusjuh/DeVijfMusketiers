using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    public bool active;
    public Transform shadowPrefab;
    bool up;
    int count;
    float timeLeft;
    Quaternion rot;

	// Use this for initialization
	void Start () {
        active = false;
        timeLeft = 0.0f;
        rot = gameObject.transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (active)
        {
            Monster[] monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
            if (monsters.Length > 0)
            {
                return;
            }
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                Vector3 pos = transform.position;
                pos.y = 0;
                //init shadow prefab
                var shadow = Instantiate(shadowPrefab, pos, Quaternion.identity);
                Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                var selectedBed = manager.selectBed();
                shadow.GetComponent<Shadow>().Init(selectedBed);
                active = false;
                //gameObject.transform.rotation = rot;
            }
            if (up)
            {
                transform.Translate(0.02f, 0, 0.02f);
                count--;
                if(count <= 0)
                {
                    up = false;
                    count = 5;
                }
            }
            else
            {
                transform.Translate(-0.02f, 0, -0.02f);
                count--;
                if (count <= 0)
                {
                    up = true;
                    count = 5;
                }
            }
            //gameObject.transform.Rotate(0, 1, 0);
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
