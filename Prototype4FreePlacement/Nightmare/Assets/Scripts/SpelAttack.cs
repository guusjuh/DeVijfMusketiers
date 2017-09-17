using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpelAttack : MonoBehaviour {
    public float cooldownTime;
    public float cooldown;
    public GameObject figure;
    public GameObject circle;
    public bool withFigure = false;
    public bool done = false;

    // Use this for initialization
    void Start ()
    {
        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0)
        {
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f, 1);
            cooldown -= Time.deltaTime;
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        }
    }

    void OnMouseDown()
    {
        if (!circle.GetComponent<SpellPlacement>().spellAreaChoosing)
        {
            done = false;
            if (cooldown <= 0)
            {
                cooldown = cooldownTime;
                if (withFigure)
                {
                    figure.SetActive(true);
                    figure.GetComponent<Monster>().startSpel(gameObject.name);
                }
                else
                {
                    done = true;
                }
            }
        }        
    }
    void OnGUI()
    {
        if (Event.current.button == 0)
        {
            if (Event.current.type == EventType.MouseUp && done)
            {
                done = false;
                Debug.Log("fingerlifted");
                StartCoroutine(StartSpellDingie());

            }
        }
    }

    public IEnumerator StartSpellDingie()
    {
        yield return new WaitForSeconds(0.1f);
        
        circle.GetComponent<SpellPlacement>().ChooseSpellPlace(gameObject.name, 0.1f);
    
    }
}
