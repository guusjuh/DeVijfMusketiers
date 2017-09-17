using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissClick : MonoBehaviour {

    public Monster attack;
    public Monster shield;
    public Monster repair;
    public Bed [] bed;

    void OnMouseDown()
    {
        Shadow target = FindObjectOfType(typeof(Shadow)) as Shadow;
        Bed[] target1 = FindObjectsOfType(typeof(Bed)) as Bed[];
        Shake[] target2 = FindObjectsOfType(typeof(Shake)) as Shake[];

        if (attack.isYellow)
        {
            target.GetComponent<Renderer>().material.color = Color.white;
            attack.isYellow = false;
        }
        
        if (shield.isYellow)
        {
            for (int i = 0; i < target1.Length; i++)
            {
                target1[i].GetComponent<Renderer>().material.color = Color.white;
            }
            shield.isYellow = false;
        }

        if (repair.isYellow)
        {
            for (int i = 0; i < target1.Length; i++)
            {
                target1[i].GetComponent<Renderer>().material.color = Color.white;
            }
            repair.isYellow = false;
        }
    }
}
