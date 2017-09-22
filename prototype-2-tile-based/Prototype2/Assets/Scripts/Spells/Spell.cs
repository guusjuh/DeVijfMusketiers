using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum SpellTypes
    {
        Attack,
        Protection,
        Repair,
        Push
    }

    private static Color attackColor = new Color(1.0f, 0.23f, 0.21f, 1.0f);
    private static Color invisColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
    private static Color pushColor = new Color(0.98f, 1.0f, 0.0f, 1.0f);
    private static Color repairColor = new Color(1.0f, 0.55f, 0.0f, 1.0f);

    private List<SpellPart> spellParts = new List<SpellPart>();
    private float timeLeft;
    public SpellTypes spellType;
    private GameObject source;

    private SpellButton bttn;
    private int cost = 1;

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
            if(source == null) Debug.LogError("Cannot cast spell on nothing?!");

            int select;

            // apply the spell effect
            switch (spellType)
            {
                case SpellTypes.Attack:
                    Debug.Log("Attack!");
                    source.GetComponent<Creature>().Hit(10);
                    break;

                case SpellTypes.Protection:
                    Debug.Log("Protect!");

                    source.GetComponent<Human>().Shield();
                    //List<Human> humans = new List<Human>();
                    //humans.AddMultiple(FindObjectsOfType(typeof(Human)) as Human[]);
                    //select = Random.Range(0, humans.FindAll(h => !h.Shielded).Count);
                    //humans.FindAll(h => !h.Shielded)[select].Shield();

                    break;
                
                case SpellTypes.Repair:
                    Debug.Log("Repair!");

                    source.GetComponent<Barrel>().Destroyed = false;

                    //List<Vase> vases = new List<Vase>();
                    //vases.AddMultiple(FindObjectsOfType(typeof(Vase)) as Vase[]);
                    //select = Random.Range(0, vases.FindAll(v => v.Destroyed).Count);
                    //vases.FindAll(v => v.Destroyed)[select].Destroyed = false;
                    break;

                case SpellTypes.Push:
                    Debug.Log("Push!");
                    if (source.GetComponent<Damagable>().type == DamagableType.Human)
                    {
                        source.GetComponent<Human>().SetSurroundingHighlight(true, bttn);
                    }
                    else
                    {
                        source.GetComponent<Barrel>().SetSurroundingHighlight(true, bttn);
                    }
                    break;

                default:
                    break;
            }

            GameManager.Instance.EndPlayerTurn(cost);

            // set to false, since done with spell
            spellParts.HandleAction(s => s.Reset());
            gameObject.SetActive(false);
        }

        // if the player didn't complete the spell in time, end spell
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            GameManager.Instance.EndPlayerTurn(cost);
            spellParts.HandleAction(s => s.Reset());
            gameObject.SetActive(false);
        }
    }

    public void StartSpell(int cost, GameObject source, SpellButton bttn, bool secondTarget)
    {
        this.source = source;

        if (secondTarget)
        {
            if (source.GetComponent<HighlightButton>().Source.GetComponent<Damagable>().type == DamagableType.Human)
            {
                source.GetComponent<HighlightButton>()
                    .Source.GetComponent<Human>()
                    .Move((int) source.GetComponent<HighlightButton>().x, (int) source.GetComponent<HighlightButton>().y);
            }
            else
            {
                source.GetComponent<HighlightButton>()
                    .Source.GetComponent<Barrel>()
                    .Move((int) source.GetComponent<HighlightButton>().x, (int) source.GetComponent<HighlightButton>().y);
            }

            // if second, than source is the target button and HIS source is the human to target
            spellParts.HandleAction(s => s.Reset());
            gameObject.SetActive(false);
            return;
        }

        this.cost = cost;
        this.bttn = bttn;

        timeLeft = 30.0f;

        spellParts.Clear();
        spellParts.AddMultiple(FindObjectsOfType(typeof(SpellPart)) as SpellPart[]);

        Color color;
        switch (spellType)
        {
            case SpellTypes.Attack:
                color = attackColor;
                break;
            case SpellTypes.Protection:
                color = invisColor;
                break;
            case SpellTypes.Push:
                color = pushColor;
                break;
            case SpellTypes.Repair:
                color = repairColor;
                break;
            default:
                color = attackColor;
                break;
        }

        for (int i = 0; i < spellParts.Count; i++)
        {
            spellParts[i].Initialize(color);
        }
    }
}
