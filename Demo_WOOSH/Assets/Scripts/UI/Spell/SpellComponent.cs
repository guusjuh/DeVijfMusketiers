﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellComponent : ISpell
{
    protected int cost;
    protected int damage;
    protected float hitChance;
    protected int fireDamage;
    protected int fireTurns;
    protected int freezeTurns;
    protected bool isDirect;
    protected int range;

    public SpellComponent()
    {
        cost = 0;
        damage = 0;
        hitChance = 1.0f;
        fireDamage = 0;
        fireTurns = 0;
        freezeTurns = 0;
        isDirect = true;
        range = 0;
    }

    public SpellComponent(int cost = 0, bool isDirect = true, int damage = 0, float hitChance = 1.0f, int fireDamage = 0, int fireTurns = 0, int freezeTurns = 0, int range = 0)
    {
        this.cost = cost;
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
        ApplyEffects(target, rnd);
        if (damage > 0 && isDirect)
            Execute(target, rnd, true);

    }

    public virtual bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        //-damage enemy
        if (ApplyEffects(target, rnd))
        {
            if (Damage() > 0)
                target.TryHit(Damage());

            if (endTurn)
                UberManager.Instance.GameManager.LevelManager.EndPlayerMove(Cost());
            return true;
        }
        if (endTurn)
            UberManager.Instance.GameManager.LevelManager.EndPlayerMove(Cost());
        return false;
    }

    public int Cost() { return cost; }
    public int Damage() { return damage; }
    public float HitChance() { return hitChance;}
    public int FireDamage() { return fireDamage; }
    public int FireTurns() { return fireTurns; }
    public int FreezeTurns() { return freezeTurns; }
    public bool IsDirect() { return isDirect; }
    public int Range() { return range; }
}
