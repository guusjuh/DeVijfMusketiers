using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour {
    public bool active;
    public Transform shadowPrefab;
    bool up;
    int count;
    float timeLeft;
    public bool selected = false;
    Quaternion rot;
    public bool destroyed;
    public Material broken;
    public Material normal;
    public Monster monster;



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
        if (destroyed && !selected)
        {
            GetComponent<Renderer>().material = broken;
        }
        else if (!monster.isYellow)
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
	}

    public void ResetMat()
    {
        
        GetComponent<Renderer>().material = (destroyed)?broken:normal;
    }

    void OnMouseDown()
    {
        if (monster.isYellow)
        {
            GetComponent<Renderer>().material = normal;
            global::Shake[] vases = FindObjectsOfType<Shake>();
            SpelAttack.disabled = false;
            destroyed = false;
            for (int i = 0; i < vases.Length; i++)
            {
                if (vases[i].destroyed)
                {
                    vases[i].ResetMat();
                }
                if (vases[i].selected)
                {
                    selected = false;
                }
            }
            monster.isYellow = false;
        }
    }
}
