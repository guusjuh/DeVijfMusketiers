using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    [SerializeField] private int totalCooldown;
    private int currentCooldown;

    private bool active = true;
    private bool canCast = true;

    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            UpdateEnable();
        }
    }

    private void UpdateEnable()
    {
        if (active && canCast && currentCooldown <= 0)
        {
            GetComponent<Button>().enabled = true;
            GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        }
        else 
        {
            GetComponent<Button>().enabled = false;
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
    }

    [SerializeField] private GameObject spellFigure;

	// Use this for initialization
	void Start ()
	{
	    currentCooldown = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!active) return;

	    canCast = CanCast();
	    UpdateEnable();
	}

    public void EndPlayerTurn()
    {
        if (currentCooldown > 0)
        {
            currentCooldown--;
        }

        UpdateEnable();
    }

    private bool CanCast()
    {
        if (currentCooldown > 0)
            return false;

        bool temp = false;

        switch (spellFigure.GetComponent<Spell>().spellType)
        {
            case Spell.SpellTypes.Attack:
                temp = true;
                break;

            // no attack case, since you can always try to kill the creature
            case Spell.SpellTypes.Protection:
                List<Human> humans = new List<Human>();
                humans.AddMultiple(FindObjectsOfType(typeof(Human)) as Human[]);

                for (int i = 0; i < humans.Count; i++)
                {
                    if (!humans[i].Shielded)
                    {
                        temp = true;
                    }
                }

                break;

            case Spell.SpellTypes.Repair:
                List<Vase> vases = new List<Vase>();
                vases.AddMultiple(FindObjectsOfType(typeof(Vase)) as Vase[]);
                for (int i = 0; i < vases.Count; i++)
                {
                    if (vases[i].Destroyed)
                    {
                        temp = true;
                    }
                }
                break;

            default:
                break;
        }

        return temp;
    }

    public void ButtonClick()
    {
        GameManager.Instance.DeactivateButtons();

        switch (spellFigure.GetComponent<Spell>().spellType)
        {
            case Spell.SpellTypes.Attack:
                HighlightBeast();
                break;

            // no attack case, since you can always try to kill the creature
            case Spell.SpellTypes.Protection:
                HighlightHuman();
                break;

            case Spell.SpellTypes.Repair:
                HighlightVase();
                break;

            default:
                break;
        }
    }

    public void CastSpell(GameObject source)
    {
        if (currentCooldown <= 0)
        {
            currentCooldown = totalCooldown;
            spellFigure.SetActive(true);
            spellFigure.GetComponent<Spell>().StartSpell(source);
        }

        DeHighlight();
    }

    void HighlightBeast()
    {
        GameObject.FindObjectOfType<Creature>().SetHighlight(true, this);
    }

    void HighlightHuman()
    {
        List<Human> possibleTargets = new List<Human>();
        Human[] tempTargets = FindObjectsOfType(typeof(Human)) as Human[];
        possibleTargets.AddMultiple(tempTargets);

        possibleTargets.HandleAction(p => p.SetHighlight(true, this));
    }

    void HighlightVase()
    {
        List<Vase> possibleTargets = new List<Vase>();
        Vase[] tempTargets = FindObjectsOfType(typeof(Vase)) as Vase[];
        possibleTargets.AddMultiple(tempTargets);

        for (int i = 0; i < possibleTargets.Count; i++)
        {
            if (possibleTargets[i].Destroyed)
                possibleTargets[i].SetHighlight(true, this);
        }
    }

    void DeHighlight()
    {
        GameObject.FindObjectOfType<Creature>().SetHighlight(false, this);

        List<Human> possibleTargets = new List<Human>();
        Human[] tempTargets = FindObjectsOfType(typeof(Human)) as Human[];
        possibleTargets.AddMultiple(tempTargets);

        possibleTargets.HandleAction(p => p.SetHighlight(true, this));

        List<Vase> possibleTargets2 = new List<Vase>();
        Vase[] tempTargets2 = FindObjectsOfType(typeof(Vase)) as Vase[];
        possibleTargets2.AddMultiple(tempTargets2);

        possibleTargets.HandleAction(v => v.SetHighlight(false, this));
        possibleTargets2.HandleAction(p => p.SetHighlight(false, this));
    }
}
