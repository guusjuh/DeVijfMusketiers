using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeComponent : SpellComponent {
    
    public FreezeComponent(int cost, SpellManager.SpellType type, bool isDirect, int freezeTurns, float freezeChance)
    {
        this.cost = cost;
        this.type = type;
        this.isDirect = isDirect;
        this.hitChance = freezeChance;
        this.freezeTurns = freezeTurns;
    }

    public override bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        if (base.Execute(target, rnd, endTurn))
        {
            if (target is Enemy)
                ((Enemy)target).Slow(freezeTurns);
            return true;
        }
        return false;
    }
}
