using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public Shake[] shakeObjects;
    public Bed[] beds;
    public float timeLeft;
    Monster monster;

    // Use this for initialization
    void Start ()
    {
        loadObjectsFromScene();
        monster.gameObject.SetActive(false);
        timeLeft = 3.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (beds.Length == 0)
        {
            Application.LoadLevel("GameOver");
        }
        if (monster.gameObject.active)
        {
            return;
        }
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
        {
            int selectShake = Random.Range(0, shakeObjects.Length);
            shakeObjects[selectShake].startShake();
            timeLeft = 5;
        }
	}

    void loadObjectsFromScene()
    {
        shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
        beds = FindObjectsOfType(typeof(Bed)) as Bed[];
        monster = FindObjectOfType(typeof(Monster)) as Monster;
    }

    public void introduceMonster()
    {
        monster.gameObject.SetActive(true);
        monster.startMonster();
    }

    public void destroyBed()
    {
        if (beds.Length == 0)
        {
            return;
        }
        int selectBed = Random.Range(0, beds.Length);
        Bed selected = beds[selectBed];

        var foos = new List<Bed>(beds);
        foos.Remove(selected);
        beds = foos.ToArray();

        Destroy(selected.gameObject);
    }
}
