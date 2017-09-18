﻿using System.Collections;
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
    public float decreasingTransparansy;
    private float t = 0;
    private float duration = 1.5f;
    private bool canDoOnce = true;
    public int lastActionChosen;
    public int wholejars;
    private GameObject lastTargetJar;
    private Bed lastTargetHuman;
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
        ShieldTimer -= Time.deltaTime;
        if (health <= 0)
        {
            SceneManager.LoadScene("Win");
        }
        Monster[] monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
        if (monsters.Length > 0)
        {
            return;
        }
        if (ShieldTimer > 0)
        {
            if (ShieldTimer >= 14)
            {
                t = 0;
            }
            GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.25f, 0.25f, 1, 1f), defaultColor, t);
            if (t < 1)
            {
                t += Time.deltaTime / ShieldTimer;
            }
        }
        else
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
        Vector3 dir;
        if (actionChosen == 0)
        {
            Shake[] shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
            for(int i = 0; i < shakeObjects.Length; i++)
            {
                if (!shakeObjects[i].GetComponent<Shake>().destroyed)
                {
                    wholejars++;
                }
            }
            if(wholejars == 0)
            {
                findTarget();
            }
            
            if (targetDestructable == null)
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
            wholejars = 0;
        }
        else
        {
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
                    if (targetBed.ShieldTimer > 0)
                    {
                        targetBed.ShieldTimer = 0;
                    }
                    else
                    {
                        manager.destroyBed(targetBed);
                    }
                    if(manager.beds.Length == 0)
                    {
                        SceneManager.LoadScene("GameOver");
                    }

                    findTarget();
                }
            }
            
        }
	}

    public void findTarget()
    {
        try
        {
            Bed[] bedObjects = FindObjectsOfType(typeof(Bed)) as Bed[];
            Shake[] shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
            wholejars = 0;
            for (int i = 0; i < shakeObjects.Length; i++)
            {
                if (!shakeObjects[i].GetComponent<Shake>().destroyed)
                {
                    wholejars++;
                }
            }

            if (wholejars == 0)
            {
                actionChosen = 1;
            }
            else if (FindObjectOfType<Manager>().beds.Length == 1 && wholejars == 1 && lastActionChosen == 1)
            {
                actionChosen = 0;
            }
            else if (lastActionChosen == 1 && wholejars >= 2)
            {
                actionChosen = 0;
            }
            else
            {
                actionChosen = Random.Range(0, 2);
            }
            lastActionChosen = actionChosen;
            


            if (actionChosen == 0)
            {
                shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
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
            else if (actionChosen == 1)
            {
                Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                
                if (manager.beds.Length > 0)
                {
                    int selectBed = Random.Range(0, manager.beds.Length);
                    targetBed = manager.beds[selectBed];

                    if (lastTargetHuman == manager.beds[selectBed] && wholejars != 0)
                    {
                        shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
                        int selectShake = Random.Range(0, shakeObjects.Length);
                        targetDestructable = shakeObjects[selectShake];
                        if (targetDestructable.destroyed)
                        {
                            findTarget();
                        }
                    }
                    else
                    {
                        targetBed = manager.beds[selectBed];
                        lastTargetHuman = targetBed;
                    }
                }
                else
                {
                    findTarget();
                }
            }
            else
            {
                findTarget();
            }
        }
        catch
        {
            SceneManager.LoadScene("Win");
        }
        }
        
}
