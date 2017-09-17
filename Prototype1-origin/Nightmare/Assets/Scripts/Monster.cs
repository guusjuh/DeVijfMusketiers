using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Draw[] draw;
    float timeLeft;
    Bed selectedBed;
    public int effect;
    public bool isYellow = false;
    public Shadow shadow;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        int selectedCount = 0;
        for (int x = 0; x < draw.Length; x++)
        {
            if (draw[x].selected)
            {
                selectedCount++;
            }
        }
        if (selectedCount == draw.Length)
        {
            //TO DO
            //spell effect
            if(effect == 0)
            {
                Shadow target = FindObjectOfType(typeof(Shadow)) as Shadow;
                target.GetComponent<Renderer>().material.color = Color.yellow;
                isYellow = true;
            }
            else if (effect == 1)
            {
                //activate shield;
                Bed[] target = FindObjectsOfType(typeof(Bed)) as Bed[];
                isYellow = true;
                for (int i = 0; i < target.Length; i++)
                {
                    target[i].GetComponent<Renderer>().material.color = Color.yellow;
                }
                //int select = Random.Range(0, target.Length);
                //target[select].ShieldTimer = 15;
            }
            else
            {
                //repair
                Shake[] target = FindObjectsOfType(typeof(Shake)) as Shake[];

                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i].destroyed)
                    {
                        target[i].GetComponent<Renderer>().material.color = Color.red;
                        isYellow = true;
                        target[i].destroyed = false;
                    }
                }
            }
            this.gameObject.SetActive(false);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void startSpel()
    {
        timeLeft = 3;
        draw = FindObjectsOfType(typeof(Draw)) as Draw[];
        for (int x = 0; x < draw.Length; x++)
        {
            draw[x].init();
        }
    }
}
