using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    Bed targetBed;
    Shake targetDestructable;
    public Monster monster;
    int actionChosen;
    public int health;

    float speed = 1.5f;
    [SerializeField] private Material normalMat;

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

        Vector3 dir;
        if (actionChosen == 0)
        {
            if(targetDestructable == null || targetDestructable.destroyed == true)
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
            if (dir.magnitude <= 1.0f)
            {
                Manager manager = FindObjectOfType(typeof(Manager)) as Manager;
                if (targetBed.ShieldTimer > 0.0f)
                {
                    if (targetBed.ShieldTimer - 5.0f <= 0.0f)
                    {
                        targetBed.ShieldTimer = 0.0f;
                    }
                    else
                    {
                        targetBed.ShieldTimer -= 5.0f;
                    }
                }
                else
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

    public void ResetMat()
    {
            GetComponent<Renderer>().material = normalMat;
    }

    void OnMouseDown()
    {
        if(monster.isYellow)
        {
            health -= 10;
            monster.isYellow = false;
            SpelAttack.disabled = false;
            ResetMat();
        }
    }
}
