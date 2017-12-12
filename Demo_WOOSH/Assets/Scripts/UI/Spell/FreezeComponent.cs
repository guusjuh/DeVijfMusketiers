using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeComponent : SpellComponent {
    
    public FreezeComponent(int freezeTurns, float hitChance)
    {
        this.hitChance = hitChance;
        this.freezeTurns = freezeTurns;
    }

    public override bool ApplyEffects(WorldObject target, float rnd)
    {
        if (base.ApplyEffects(target, rnd))
        {
            ((Enemy) target).Slow(freezeTurns);
            return true;
        } else
        {
            return false;
        }
    }
}
