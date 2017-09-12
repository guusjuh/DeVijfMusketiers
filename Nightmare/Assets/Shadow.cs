using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    Bed targetBed;
    float speed = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Monster[] monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
        if (monsters.Length > 0)
        {
            return;
        }
        if(targetBed == null)
        {
            Destroy(this.gameObject);
            return;
        }
        Vector3 dir;
        dir = targetBed.transform.position - gameObject.transform.position;
        gameObject.transform.Translate(dir.normalized * Time.deltaTime * speed);
        if(dir.magnitude < 1)
        {
            Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
            manager.introduceMonster(targetBed);
            Destroy(this.gameObject);
        }
	}

    public void Init(Bed target)
    {
        targetBed = target;
    }
}
