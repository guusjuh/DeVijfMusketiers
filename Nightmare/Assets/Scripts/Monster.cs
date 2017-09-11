using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Draw[] draw;
    float timeLeft;

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
            this.gameObject.SetActive(false);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
            manager.destroyBed();
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
