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
        SpelAttack.disabled = false;
        Shake[] vases = FindObjectsOfType<Shake>();
        Bed[] beds = FindObjectsOfType<Bed>();
        Shadow beast = FindObjectOfType<Shadow>();
        /*for (int i = 0; i < vases.Length; i++)
        {
            if (vases[i].destroyed)
            {
                vases[i].ResetMat();
            }
        }
        for (int i = 0; i < beds.Length; i++)
        {
            beds[i].ResetMat();
        }
        beast.ResetMat();*/


        Shadow target = FindObjectOfType(typeof(Shadow)) as Shadow;
        Bed[] target1 = FindObjectsOfType(typeof(Bed)) as Bed[];
        Shake[] target2 = FindObjectsOfType(typeof(Shake)) as Shake[];

        if (attack.isYellow)
        {
            target.GetComponent<Renderer>().material.color = Color.white;
            beast.ResetMat();
            attack.isYellow = false;
        }
        
        if (shield.isYellow)
        {
            for (int i = 0; i < target1.Length; i++)
            {
                target1[i].GetComponent<Renderer>().material.color = Color.white;
            }
            for (int i = 0; i < beds.Length; i++)
            {
                beds[i].ResetMat();
            }

            shield.isYellow = false;
        }

        if (repair.isYellow)
        {
            for (int i = 0; i < target2.Length; i++)
            {
                target2[i].GetComponent<Renderer>().material.color = Color.white;
                target2[i].ResetMat();
            }
            /*for (int i = 0; i < vases.Length; i++)
            {
                if (vases[i].destroyed)
                {
                    vases[i].ResetMat();
                }
            }*/
            repair.isYellow = false;
        }
    }
}
