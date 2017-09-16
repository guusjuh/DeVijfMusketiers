using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    // singleton
    private static SpellManager instance = null;
    public int attackDamage = 10; //for attack see highlight class D:

    public static SpellManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType(typeof(SpellManager)) as SpellManager;
            return instance;
        }
    }

    private Queue playerQueue;
    private Queue beastQueue;

    public void Start()
    {
        playerQueue = gameObject.transform.GetChild(0).GetComponent<Queue>();
        beastQueue = gameObject.transform.GetChild(1).GetComponent<EnemyQueue>();
        HighlightTargets();
    }

    public void HighlightTargets()
    {
        PlayerSpell activeSpell = playerQueue.Peek();
        switch (activeSpell)
        {
              case PlayerSpell.attack:
                HighlightBeast();
                break;
            case PlayerSpell.repair:
                HighlightVases();
                break;
            case PlayerSpell.shield:
                HighlightHumans();
                break;
            default:
                break;
        }
    }

    private void HighlightBeast(bool highLightValue = true)
    {
        GameObject beast = GameObject.FindGameObjectWithTag("Beast");
        beast.GetComponent<Beast>().SetHighlight(highLightValue);
    }

    private void HighlightHumans(bool highLightValue = true)
    {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].GetComponent<Human>().SetHighlight(highLightValue);
        }
    }

    private void HighlightVases(bool highLightValue = true)
    {
        GameObject[] vases = GameObject.FindGameObjectsWithTag("Vase");
        for (int i = 0; i < vases.Length; i++)
        {
            if (vases[i].GetComponent<Vase>().destroyed)
            {
                vases[i].GetComponent<Vase>().SetHighlight(highLightValue);
            }
        }
    }

    public void RemoveHighlights()
    {
        HighlightBeast(false);
        HighlightHumans(false);
        HighlightVases(false);
        HighlightTargets();
    }

    public void AddShieldSpell()
    {
        playerQueue.Add((int)PlayerSpell.shield);
    }

    public void AddRepairSpell()
    {
        playerQueue.Add((int)PlayerSpell.repair);
    }

    public void AddAttackSpell()
    {
        playerQueue.Add((int)PlayerSpell.attack);
    }

    public void AddHumanSpell()
    {
        beastQueue.Add((int)EnemySpells.human);
    }

    public void AddVaseSpell()
    {
        beastQueue.Add((int)EnemySpells.vase);
    }

    public void PlayerQueueRemove(int id)
    {
        if (id == 0)
        {
            playerQueue.Remove();
            RemoveHighlights();
            HighlightTargets();
        } else if (id > 0)
        {
            playerQueue.RemoveAt(id);
        }
    }

    public void EnemyQueueRemove()
    {
        beastQueue.Remove();
    }

    public void Update()
    {
        HighlightTargets();
    }
}

public enum PlayerSpell
{
    unknown = -1,
    attack = 0,
    shield = 1,
    repair = 2
}

public enum EnemySpells
{
    human = 0,
    vase = 1
}

