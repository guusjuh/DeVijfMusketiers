using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour
{
    [SerializeField] private int totalCooldown;
    private int currentCooldown;
    [SerializeField] private int cost = 1;

    private bool active = true;
    private bool canCast = true;

    [SerializeField]
    private GameObject spellFigure;

    [SerializeField]
    private GameObject disabled;
    [SerializeField]
    private Text cooldownText;

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
            disabled.SetActive(false);

            //GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        }
        else 
        {
            GetComponent<Button>().enabled = false;
            disabled.SetActive(true);
            cooldownText.text = currentCooldown > 0 ? ""+currentCooldown : "";
            //GetComponent<UnityEngine.UI.Image>().color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
    }


	// Use this for initialization
	void Start ()
	{
	    currentCooldown = 0;

	    //disabled = GetComponent<RectTransform>().GetChild(1).gameObject;
	    //cooldownText = disabled.GetComponentInChildren<Text>();
	    cooldownText.text = "";
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
            cooldownText.text = "" + currentCooldown;
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
                    temp = true;
                }

                break;

            case Spell.SpellTypes.Repair:
                List<Barrel> vases = new List<Barrel>();
                vases.AddMultiple(FindObjectsOfType(typeof(Barrel)) as Barrel[]);
                for (int i = 0; i < vases.Count; i++)
                {
                    if (vases[i].Destroyed)
                    {
                        temp = true;
                    }
                }
                break;

            case Spell.SpellTypes.Push:
                temp = true; // cuz you can always push, else u dead.

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
                HighlightBrokenBarrel();
                break;

            case Spell.SpellTypes.Push:
                HighlightHuman();
                HighlightBarrel();
                break;

            default:
                break;
        }
    }

    public void CastSpell(GameObject source, bool secondTarget = false)
    {
        if (currentCooldown <= 0)
        {
            currentCooldown = totalCooldown;
            spellFigure.SetActive(true);
            spellFigure.GetComponent<Spell>().StartSpell(cost, source, this, secondTarget);
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

    void HighlightBrokenBarrel()
    {
        List<Barrel> possibleTargets = new List<Barrel>();
        Barrel[] tempTargets = FindObjectsOfType(typeof(Barrel)) as Barrel[];
        possibleTargets.AddMultiple(tempTargets);

        for (int i = 0; i < possibleTargets.Count; i++)
        {
            if (possibleTargets[i].Destroyed)
                possibleTargets[i].SetHighlight(true, this);
        }
    }

    void HighlightBarrel()
    {
        List<Barrel> possibleTargets = new List<Barrel>();
        Barrel[] tempTargets = FindObjectsOfType(typeof(Barrel)) as Barrel[];
        possibleTargets.AddMultiple(tempTargets);

        for (int i = 0; i < possibleTargets.Count; i++)
        {
            if (!possibleTargets[i].Destroyed)
                possibleTargets[i].SetHighlight(true, this);
        }
    }

    void DeHighlight()
    {
        // dehighlight creature
        GameObject.FindObjectOfType<Creature>().SetHighlight(false, this);

        // humans
        List<Human> possibleTargets = new List<Human>();
        Human[] tempTargets = FindObjectsOfType(typeof(Human)) as Human[];
        possibleTargets.AddMultiple(tempTargets);
        possibleTargets.HandleAction(v => v.SetHighlight(false, this));

        // barrels
        List<Barrel> possibleTargets2 = new List<Barrel>();
        Barrel[] tempTargets2 = FindObjectsOfType(typeof(Barrel)) as Barrel[];
        possibleTargets2.AddMultiple(tempTargets2);
        possibleTargets2.HandleAction(p => p.SetHighlight(false, this));
    }
}
