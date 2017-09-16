using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    bool up;
    int count;
    float timeLeft;
    Quaternion rot;
    public bool destroyed;
    public Material broken;
    public Material normal;
    public GameObject button;
    public GameObject circle;

    // Use this for initialization
    void Start () {
        destroyed = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (destroyed)
        {
            GetComponent<Renderer>().material = broken;
            button.SetActive(true);
            circle.SetActive(true);
        }
        else
        {
            GetComponent<Renderer>().material = normal;
            button.SetActive(false);
            circle.SetActive(false);
        }
	}
}
