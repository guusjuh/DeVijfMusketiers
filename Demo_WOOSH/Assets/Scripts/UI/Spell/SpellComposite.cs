using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellComposite : ISpell
{
    private List<SpellComponent> components;

    public SpellComposite(List<SpellComponent> components)
    {
        this.components = components;
    }

    public SpellComposite AddComponent(SpellComponent component)
    {
        components.Add(component);
        return this;
    }

    /// <summary>
    /// deals the total damage to the target
    /// </summary>
    public void Execute(WorldObject target)
    {
        //TODO: check for hitchance
        int damage = 0;
        for (int i = 0; i < components.Count; i++)
        {
            damage += components[i].Damage();
        }
        target.TryHit(damage);
    }

    /// <summary>
    /// activates the effects of each spell and prepares for execution
    /// </summary>
    public void CastSpell(WorldObject target)
    {
        for(int i = 0; i < components.Count; i++)
        {
            components[i].ApplyEffects(target);
        }
        if (IsDirect())
            Execute(target);
    }

    public bool ApplyEffects(WorldObject target)
    {
        bool totalSucces = true;
        for(int i =0; i<components.Count; i++)
        {
            if (!components[i].ApplyEffects(target))
            {
                totalSucces = false;
            }
        }
        return totalSucces;
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
}
