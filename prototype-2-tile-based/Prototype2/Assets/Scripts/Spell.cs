﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum SpellTypes
    {
        Attack,
        Protection,
        Repair
    }

    private List<SpellPart> spellParts = new List<SpellPart>();
    private float timeLeft;
    public SpellTypes spellType;

	// Update is called once per frame
	void Update () {
        // check for all spellparts being completed
        int completedCount = 0;
        for (int x = 0; x < spellParts.Count; x++)
        {
            if (spellParts[x].completed)
            {
                completedCount++;
            }
        }

        // if the spell is completed
        if (completedCount == spellParts.Count)
        {
            int select;

            // apply the spell effect
            switch (spellType)
            {
                case SpellTypes.Attack:
                    Debug.Log("Attack!");
                    GameManager.Instance.Creature.Hit(50);
                    break;

                case SpellTypes.Protection:
                    Debug.Log("Protect!");
                    //TODO: target selection
                    List<Human> humans = new List<Human>();
                    humans.AddMultiple(FindObjectsOfType(typeof(Human)) as Human[]);
                    select = Random.Range(0, humans.FindAll(h => !h.Shielded).Count);
                    StartCoroutine(humans.FindAll(h => !h.Shielded)[select].Shield());
                    break;
                
                case SpellTypes.Repair:
                    Debug.Log("Repair!");
                    //TODO: target selection
                    List<Vase> vases = new List<Vase>();
                    vases.AddMultiple(FindObjectsOfType(typeof(Vase)) as Vase[]);
                    select = Random.Range(0, vases.FindAll(v => v.Destroyed).Count);
                    vases.FindAll(v => v.Destroyed)[select].Destroyed = false;
                    break;

                default:
                    break;
            }

            // set to false, since done with spell
            spellParts.HandleAction(s => s.Reset());
            gameObject.SetActive(false);
        }

        // if the player didn't complete the spell in time, end spell
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartSpell()
    {
        timeLeft = 3.0f;

        spellParts.Clear();
        spellParts.AddMultiple(FindObjectsOfType(typeof(SpellPart)) as SpellPart[]);

        for (int i = 0; i < spellParts.Count; i++)
        {
            spellParts[i].Initialize();
        }
    }
}
