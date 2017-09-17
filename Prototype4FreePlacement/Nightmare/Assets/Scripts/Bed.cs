using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour {
    public float ShieldTimer;
    Color defaultColor;
    public bool canBeAttacked = false;

	// Use this for initialization
	void Start () {
        defaultColor = GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
        if(ShieldTimer >= 0)
        {
            ShieldTimer -= Time.deltaTime;
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1, 1);
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
	}
}
