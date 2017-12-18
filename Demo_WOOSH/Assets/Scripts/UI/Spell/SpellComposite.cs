using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellComposite : ISpell
{
    private List<ISpell> components;

    public SpellComposite(List<ISpell> components)
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
    public bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        int damage = 0;
        for (int i = 0; i < components.Count; i++)
        {
            if (components[i].Damage() > 0 && rnd <= components[i].HitChance())
                damage += components[i].Damage();
            if (components[i].Damage() <= 0)
                components[i].Execute(target, rnd, false);
        }
        if (Damage() > 0)
            target.TryHit(damage);

        if (endTurn)
            UberManager.Instance.GameManager.LevelManager.EndPlayerMove(Cost());
        return true;
    }

    /// <summary>
    /// activates the effects of each spell and prepares for execution
    /// </summary>
    public void CastSpell(WorldObject target)
    {
        float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
        if (IsDirect())
            Execute(target, rnd, true);
        else
            HighlightTiles(target);
    }

    public bool DoesHit(WorldObject target, float rnd)
    {
        bool totalSucces = true;
        for(int i =0; i<components.Count; i++)
        {
            if (!components[i].DoesHit(target, rnd))
            {
                totalSucces = false;
            }
        }
        return totalSucces;
    }

    public void HighlightTiles(WorldObject target)
    {
        for (int i = 0; i < components.Count; i++)
        {
            components[i].HighlightTiles(target);
        }
    }

    public SpellManager.SpellType Type()
    {
        if (components.Count > 0)
        {
            SpellManager.SpellType type = components[0].Type();
            for (int i = 1; i < components.Count; i++)
            {
                if (type != components[i].Type())
                {
                    return SpellManager.SpellType.NoSpell;
                }
            }
            return type;
        }
        return SpellManager.SpellType.NoSpell;
    }

    public int Cost()
    {
        int cost = 0;
        for (int i = 0; i < components.Count; i++)
            cost += components[i].Cost();

        return cost;
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
