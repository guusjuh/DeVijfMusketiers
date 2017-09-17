using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shadow : MonoBehaviour {
    Bed targetBed;
    Shake targetDestructable;
    int actionChosen;
    public int health;
    public float ShieldTimer;
    Color defaultColor;

    float speed = 2.5f;

	// Use this for initialization
	void Start ()
    {
        defaultColor = GetComponent<Renderer>().material.color;
        findTarget();
    }
	
	// Update is called once per frame
	void Update ()
    {        
        if (health <= 0)
        {
            SceneManager.LoadScene("Win");
        }
        Monster[] monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
        if (monsters.Length > 0)
        {
            return;
        }
        if (ShieldTimer >= 0)
        {
            ShieldTimer -= Time.deltaTime;
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 1, 1);
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultColor;
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
            Bed[] bedObjects = FindObjectsOfType(typeof(Bed)) as Bed[];
            if(bedObjects.Length <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
            if (targetBed == null)
            {
                findTarget();
            }
            else
            {
                dir = targetBed.transform.position - gameObject.transform.position;
                gameObject.transform.Translate(dir.normalized * Time.deltaTime * speed);
                if (dir.magnitude < 1)
                {
                    Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                    if (targetBed.ShieldTimer >= 0)
                    {
                        targetBed.ShieldTimer -= 5;
                    }
                    else
                    {
                        manager.destroyBed(targetBed);
                    }

                    findTarget();
                }
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
            }
            else
            {
                findTarget();
            }
        }
    }
}
