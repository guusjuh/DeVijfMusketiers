using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeComponent : SpellComponent {
    
    public FreezeComponent(int freezeTurns, float hitChance)
    {
        damage = 0;
        this.hitChance = hitChance;
        fireDamage = 0;
        fireTurns = 0;
        this.freezeTurns = freezeTurns;
        isDirect = true;
        range = 0;
    }

    public override bool ApplyEffects(WorldObject target)
    {
        if (base.ApplyEffects(target))
        {
            //TODO: freeze enemy
            return true;
        } else
        {
            return false;
        }
    }
}
