using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Draw[] draw;
    float timeLeft;
    Bed selectedBed;
    public int effect;
    public bool isYellow = false;
    public Shadow shadow;

    [SerializeField] private Material[] materials = new Material[3];

    // Update is called once per frame
    void Update ()
    {
        int selectedCount = 0;
        for (int x = 0; x < draw.Length; x++)
        {
            if (draw[x].selected)
            {
                selectedCount++;
            }
        }
        if (selectedCount == draw.Length)
        {
            Time.timeScale = 1f;
            //TO DO
            //spell effect
            if (effect == 0)
            {//TARGET BOSS
                Shadow target = FindObjectOfType(typeof(Shadow)) as Shadow;
                target.GetComponent<Renderer>().material = materials[0];
                isYellow = true;
            }
            else if (effect == 1)
            {//TARGET SHIELD
                //activate shield;
                Bed[] target = FindObjectsOfType(typeof(Bed)) as Bed[];
                isYellow = true;
                for (int i = 0; i < target.Length; i++)
                {
                    target[i].GetComponent<Renderer>().material = materials[1];
                }
                //int select = Random.Range(0, target.Length);
                //target[select].ShieldTimer = 15;
            }
            else
            {//TARGET REPAIR
                //repair
                Shake[] target = FindObjectsOfType(typeof(Shake)) as Shake[];

                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i].destroyed)
                    {
                        target[i].GetComponent<Renderer>().material = materials[2];
                        target[i].selected = true;
                        isYellow = true;
                        
                    }
                }
            }
            this.gameObject.SetActive(false);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            SpelAttack.disabled = false;
            this.gameObject.SetActive(false);
        }
    }

    public void startSpel()
    {
        Time.timeScale = 0.5f;
        timeLeft = 3;
        draw = FindObjectsOfType(typeof(Draw)) as Draw[];
        for (int x = 0; x < draw.Length; x++)
        {
            draw[x].init();
        }
    }
}
