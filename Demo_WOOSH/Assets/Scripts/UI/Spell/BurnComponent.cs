using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnComponent : SpellComponent {
    //
    public BurnComponent(int cost, SpellManager.SpellType type, bool isDirect, int burnDamage, int fireTurns, float burnChance)
    {
        this.cost = cost;
        this.type = type;
        this.isDirect = isDirect;
        this.hitChance = burnChance;
        this.burnDamage = burnDamage;
        this.fireTurns = fireTurns;
    }

    public override bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        if (base.Execute(target, rnd, endTurn))
        {
            if (target is Enemy)
                ((Enemy)target).Burn(FireTurns(), FireDamage());
            return true;
        }
        return false;
    }
}
