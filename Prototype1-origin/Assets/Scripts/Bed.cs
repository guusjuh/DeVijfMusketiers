using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour {
    public float ShieldTimer;
    Color defaultColor;
    public Monster monster;

	// Use this for initialization
	void Start ()
    {
        defaultColor = GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(ShieldTimer >= 0)
        {
            ShieldTimer -= Time.deltaTime;
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1, 1);
        }
        else if(!monster.isYellow)
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
	}

    void OnMouseDown()
    {
        if (monster.isYellow)
        {
            ShieldTimer = 5;
            monster.isYellow = false;
            GetComponent<Renderer>().material.color = Color.white;
        }
        
    }
}
