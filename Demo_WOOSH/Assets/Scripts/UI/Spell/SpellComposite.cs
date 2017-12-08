using System.Collections.Generic;
using UnityEngine;

public class SpellComposite : ISpell
{
    private List<SpellComponent> components;

    public SpellComposite(List<SpellComponent> components)
    {
        this.components = components;
    }

    public int Damage()
    {
        int damage = 0;
        for (int i = 0; i < components.Count; i++)
            damage += components[i].Damage();

        return damage;
    }

    public float HitChance()
    {
        float hitChance = 0.0f;
        for (int i = 0; i < components.Count; i++)
            hitChance += components[i].HitChance();

        return hitChance;
    }

    public int FireDamage()
    {
        int fireDamage = 0;
        for (int i = 0; i < components.Count; i++)
            fireDamage += components[i].FireDamage();
        return fireDamage; 
    }

    public int FireTurns()
    {
        int fireTurns = 0;
        for (int i = 0; i < components.Count; i++)
            fireTurns += components[i].FireTurns();
        return fireTurns;
    }

    public int FreezeTurns()
    {
        int freezeTurns = 0;
        for (int i = 0; i < components.Count; i++)
            freezeTurns += components[i].FreezeTurns();
        return freezeTurns;
    }

    public bool IsDirect()
    {
        for (int i = 0; i < components.Count; i++)
            if (components[i].IsDirect()) return true;

        return false;
    }

    public int Range()
    {
        int range = 0;
        for (int i = 0; i < components.Count; i++)
            range += components[i].Range();

        return range;
    }

    public bool Excecute()
    {
        if(IsDirect())
            HighlightTiles();

        float rnd = Random.Range(0.0f, 1.0f);
        if (rnd > HitChance() && !IsDirect())
        {
            Debug.Log("I execute spell now comrade!");
            CastSpell();
            return true;
        }
        return false;
    }

    public void CastSpell()
    {
        Debug.Log("I cast spell now comrade!");
    }

    public void HighlightTiles()
    {
        Debug.Log("I highlight tiles now comrade! [" + Range() + "]");
        //TODO: wait for indirect stuff
    }
}
