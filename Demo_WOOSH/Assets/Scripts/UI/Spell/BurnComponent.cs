using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnComponent : SpellComponent {

    public BurnComponent(int fireDamage, int fireTurns, float hitChance)
    {
        damage = 0;
        this.hitChance = hitChance;
        this.fireDamage = fireDamage;
        this.fireTurns = fireTurns;
        freezeTurns = 0;
        isDirect = true;
        range = 0;
    }

    public override bool ApplyEffects(WorldObject target)
    {
        if (base.ApplyEffects(target))
        {
            //TODO: burn enemy
            ((Enemy)target).Burn(FireTurns(), FireDamage());
            return true;
        }
        else
        {
            return false;
        }
    }
}
