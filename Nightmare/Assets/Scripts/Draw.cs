using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour {
    public bool selected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void init()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 255);
        selected = false;
    }

    void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 255, 255);
            selected = true;
        }
    }
}
