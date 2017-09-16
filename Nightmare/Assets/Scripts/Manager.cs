using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    public Shake[] shakeObjects;
    public Bed[] beds;
    public float ShieldCooldown;
    public float AttackCooldown;
    public float RepairCooldown;

    // Use this for initialization
    void Start ()
    {
        loadObjectsFromScene();
        ShieldCooldown = 0;
        AttackCooldown = 0;
        RepairCooldown = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (beds.Length == 0)
        {
            Application.LoadLevel("GameOver");
        }
        float timePast = Time.deltaTime;
        ShieldCooldown -= timePast;
        AttackCooldown -= timePast;
        RepairCooldown -= timePast;
    }

    void loadObjectsFromScene()
    {
        shakeObjects = FindObjectsOfType(typeof(Shake)) as Shake[];
        beds = FindObjectsOfType(typeof(Bed)) as Bed[];
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
