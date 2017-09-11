using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Draw[] draw;
    float timeLeft;
    public GameObject dead;
    public GameObject alive;


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
            alive.SetActive(true);
            alive.GetComponent<Timer>().resetTimer();
            this.gameObject.SetActive(false);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
            manager.destroyBed();
            dead.SetActive(true);
            dead.GetComponent<Timer>().resetTimer();
            this.gameObject.SetActive(false);
        }
    }

    public void startMonster()
    {
        timeLeft = 4;
        draw = FindObjectsOfType(typeof(Draw)) as Draw[];
        for (int x = 0; x < draw.Length; x++)
        {
            draw[x].init();
        }
    }
}
