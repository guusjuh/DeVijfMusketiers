using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {
    public Shake[] shakeObjects;
    public Bed[] beds;
    Monster[] monsters;

    // Use this for initialization
    void Start ()
    {
        loadObjectsFromScene();
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i].gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (beds.Length == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
	}

    void loadObjectsFromScene()
    {
        beds = FindObjectsOfType(typeof(Bed)) as Bed[];
        shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
        monsters = FindObjectsOfType(typeof(Monster)) as Monster[];
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

}
