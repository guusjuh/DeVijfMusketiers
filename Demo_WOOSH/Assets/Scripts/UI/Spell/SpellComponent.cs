using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellComponent : ISpell
{
    protected int damage;
    protected float hitChance;
    protected int fireDamage;
    protected int fireTurns;
    protected int freezeTurns;
    protected bool isDirect;
    protected int range;

    public SpellComponent()
    {
        damage = 0;
        hitChance = 1.0f;
        fireDamage = 0;
        fireTurns = 0;
        freezeTurns = 0;
        isDirect = true;
        range = 0;
    }

    public SpellComponent(int damage = 0, float hitChance = 1.0f, int fireDamage = 0, int fireTurns = 0, int freezeTurns = 0, bool isDirect = true, int range = 0)
    {
        this.damage = damage;
        this.hitChance = hitChance;
        this.fireDamage = fireDamage;
        this.fireTurns = fireTurns;
        this.freezeTurns = freezeTurns;
        this.isDirect = isDirect;
        this.range = range;
    }

    public SpellComposite AddComponent(SpellComponent component)
    {
        List<ISpell> components = new List<ISpell>();
        components.Add(this);
        components.Add(component);
        SpellComposite composite = new SpellComposite(components);
        return composite;
    }

    public virtual bool ApplyEffects(WorldObject target, float rnd)
    {
        if (hitChance < 1.0f)
        {
            if (rnd <= hitChance)
            {
                return true;
            }

            return false;
        }

        return true;
    }

    public virtual void CastSpell(WorldObject target)
    {
        float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
        if (damage > 0)
            Execute(target, rnd);
    }

    public virtual void Execute(WorldObject target, float rnd)
    {
        //-damage enemy
        if (ApplyEffects(target, rnd))
            target.TryHit(Damage());
    }

    public int Damage() { return damage; }
    public float HitChance() { return hitChance;}
    public int FireDamage() { return fireDamage; }
    public int FireTurns() { return fireTurns; }
    public int FreezeTurns() { return freezeTurns; }
    public bool IsDirect() { return isDirect; }
    public int Range() { return range; }
}
