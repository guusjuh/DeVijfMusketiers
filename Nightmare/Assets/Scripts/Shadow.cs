using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    Bed targetBed;
    Shake targetDestructable;
    int actionChosen;
    public int health;
    public GameObject target;

    float speed = 1.5f;

	// Use this for initialization
	void Start ()
    {
        findTarget();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (health <= 0)
        {
            Application.LoadLevel("Win");
        }
        Monster[] monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
        if (monsters.Length > 0)
        {
            return;
        }
        Vector3 dir;
        if (actionChosen == 0)
        {
            if(targetDestructable == null)
            {
                findTarget();
            }
            dir = targetDestructable.transform.position - gameObject.transform.position;
            gameObject.transform.Translate(dir.normalized * Time.deltaTime * speed);
            if (dir.magnitude < 1)
            {
                targetDestructable.destroyed = true;

                findTarget();
            }
        }
        else
        {
            if (targetBed == null)
            {
                findTarget();
            }
            dir = targetBed.transform.position - gameObject.transform.position;
            gameObject.transform.Translate(dir.normalized * Time.deltaTime * speed);
            if (dir.magnitude < 1)
            {
                Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                if (targetBed.ShieldTimer <= 0)
                {
                    manager.destroyBed(targetBed);
                }

                findTarget();
            }
        }
	}

    public void findTarget()
    {
        actionChosen = Random.Range(0, 2);
        if(actionChosen == 0)
        {
            Shake[] shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
            if (shakeObjects.Length > 0)
            {
                int selectShake = Random.Range(0, shakeObjects.Length);
                targetDestructable = shakeObjects[selectShake];
                target.transform.position = new Vector3(targetDestructable.transform.position.x, 0, targetDestructable.transform.position.z);
                if (targetDestructable.destroyed)
                {
                    findTarget();
                }
            }
            else
            {
                findTarget();
            }
        }
        else
        {
            Bed[] bedObjects = FindObjectsOfType(typeof(Bed)) as Bed[];
            if (bedObjects.Length > 0)
            {
                int selectBed = Random.Range(0, bedObjects.Length);
                targetBed = bedObjects[selectBed];
                target.transform.position = new Vector3(targetBed.transform.position.x, 0, targetBed.transform.position.z);
            }
            else
            {
                findTarget();
            }
        }
    }
}
