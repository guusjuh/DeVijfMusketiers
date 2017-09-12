using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public Shake[] shakeObjects;
    public Bed[] beds;
    public float timeLeft;
    public float totalTime;
    public float timePassed;
    Monster[] monsters;
    public Light light;

    // Use this for initialization
    void Start ()
    {
        loadObjectsFromScene();
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i].gameObject.SetActive(false);
        }
        timeLeft = 3.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (beds.Length == 0)
        {
            Application.LoadLevel("GameOver");
        }
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].gameObject.active)
            {
                return;
            }
        }
        timeLeft -= Time.deltaTime;
        timePassed += Time.deltaTime;
        light.intensity = 0.5f + (Mathf.Abs(timePassed - totalTime / 2) / totalTime);
        if (totalTime < timePassed)
        {
            Application.LoadLevel("Win");
            return;
        }
        if(timeLeft <= 0)
        {
            int selectShake = Random.Range(0, shakeObjects.Length);
            shakeObjects[selectShake].startShake();
            timeLeft = Random.Range(0, 4 - ((timePassed / totalTime) * 2));
        }
	}

    void loadObjectsFromScene()
    {
        shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
        beds = FindObjectsOfType(typeof(Bed)) as Bed[];
        monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
    }

    public void introduceMonster(Bed selected)
    {
        int selectMonster = Random.Range(0, monsters.Length);
        monsters[selectMonster].gameObject.SetActive(true);
        monsters[selectMonster].startMonster(selected);
    }

    public void destroyBed(Bed selected)
    {
        if (beds.Length == 0)
        {
            return;
        }

        var foos = new List<Bed>(beds);
        foos.Remove(selected);
        beds = foos.ToArray();

        Destroy(selected.gameObject);
    }

    public Bed selectBed()
    {
        if (beds.Length == 0)
        {
            return null;
        }

        int selectBed = Random.Range(0, beds.Length);
        return beds[selectBed];
    }
}
