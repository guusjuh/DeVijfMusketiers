using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelAttack : MonoBehaviour {
    public float cooldownTime;
    float cooldown;
    public GameObject figure;

	// Use this for initialization
	void Start ()
    {
        cooldown = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(cooldown > 0)
        {
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f, 1);
            cooldown -= Time.deltaTime;
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        }
    }

    void OnMouseDown()
    {
        if (cooldown <= 0)
        {
            cooldown = cooldownTime;
            figure.SetActive(true);
            figure.GetComponent<Monster>().startSpel();
        }
    }
}
