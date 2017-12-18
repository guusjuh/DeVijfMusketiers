using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellComponent : ISpell
{
    protected SpellManager.SpellType type;
    protected int cost;
    protected int damage;
    protected float hitChance;
    protected int burnDamage;
    protected int fireTurns;
    protected int freezeTurns;
    protected bool isDirect;
    protected int range;

    public SpellComponent()
    {
        cost = 0;
        type = SpellManager.SpellType.NoSpell;
        damage = 0;
        hitChance = 1.0f;
        burnDamage = 0;
        fireTurns = 0;
        freezeTurns = 0;
        isDirect = true;
        range = 0;
    }

    public SpellComponent(int cost = 0, SpellManager.SpellType type = SpellManager.SpellType.NoSpell, bool isDirect = true, int damage = 0, float hitChance = 1.0f, int burnDamage = 0, int fireTurns = 0, int freezeTurns = 0, int range = 0)
    {
        this.cost = cost;
        this.type = type;
        this.damage = damage;
        this.hitChance = hitChance;
        this.burnDamage = burnDamage;
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

    public virtual bool DoesHit(WorldObject target, float rnd)
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
        if (isDirect)
        {
            Execute(target, rnd, true);
        }
        else
        {
            HighlightTiles(target);
        }
    }

    public virtual bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        //-damage enemy
        if (DoesHit(target, rnd))
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

    public void HighlightTiles(WorldObject target)
    {
        UberManager.Instance.GameManager.TileManager.ShowTeleportHighlights(target, Range());
        //-listen for click on certain tile
        UberManager.Instance.InputManager.highlightsActivated = true;
    }

    public SpellManager.SpellType Type() { return type; }
    public int Cost() { return cost; }
    public int Damage() { return damage; }
    public float HitChance() { return hitChance;}
    public int FireDamage() { return burnDamage; }
    public int FireTurns() { return fireTurns; }
    public int FreezeTurns() { return freezeTurns; }
    public bool IsDirect() { return isDirect; }
    public int Range() { return range; }
}
