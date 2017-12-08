using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellComponent : ISpell
{
    private int damage;
    private float hitChance;
    private int fireDamage;
    private int fireTurns;
    private int freezeTurns;
    private bool isDirect;
    private int range;

    public int Damage() { return damage; }
    public float HitChance() { return hitChance;}
    public int FireDamage() { return fireDamage; }
    public int FireTurns() { return fireTurns; }
    public int FreezeTurns() { return freezeTurns; }
    public bool IsDirect() { return isDirect; }
    public int Range() { return range; }
}
