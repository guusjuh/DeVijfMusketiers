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
    public bool destroyed;
    public Material broken;
    public Material normal;

	// Use this for initialization
	void Start () {
        active = false;
        destroyed = false;
        timeLeft = 0.0f;
        rot = gameObject.transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (destroyed)
        {
            GetComponent<Renderer>().material = broken;
        }
        else
        {
            GetComponent<Renderer>().material = normal;
        }
	}
}
