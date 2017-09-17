using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Draw[] draw;
    float timeLeft;
    Bed selectedBed;
    public int effect;
    public GameObject circle;
    public string nme;
    public bool fingerLifted;
    int selectedCount;
    public bool done = false;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        selectedCount = 0;
        for (int x = 0; x < draw.Length; x++)
        {
            if (draw[x].selected)
            {
                selectedCount++;
                if(selectedCount == draw.Length)
                {
                    done = true;
                }
            }
        }
        if (selectedCount == draw.Length)
        {
            if (fingerLifted)
            {
                //TO DO
                //spell effect
                fingerLifted = false;
                if (effect == 0)
                {
                    circle.GetComponent<SpellPlacement>().ChooseSpellPlace(nme, 0.1f);
                }
                else if (effect == 1)
                {
                    circle.GetComponent<SpellPlacement>().ChooseSpellPlace(nme, 0.1f);
                }
                else
                {
                    circle.GetComponent<SpellPlacement>().ChooseSpellPlace(nme, 0.1f);
                }
                this.gameObject.SetActive(false);
            }
            

        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (Event.current.button == 0)
        {
            if (Event.current.type == EventType.MouseUp && done)
            {
                done = false;
                fingerLifted = true;
                Debug.Log("fingerlifted");
                
            }
        }
    }

    public void startSpel(string name)
    {
        nme = name;
        timeLeft = 3;
        draw = FindObjectsOfType(typeof(Draw)) as Draw[];
        for (int x = 0; x < draw.Length; x++)
        {
            draw[x].init();
        }
    }
}
