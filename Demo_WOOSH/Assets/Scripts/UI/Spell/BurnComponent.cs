using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnComponent : SpellComponent {

    public BurnComponent(int cost, bool isDirect, int fireDamage, int fireTurns, float hitChance)
    {
        this.cost = cost;
        this.isDirect = isDirect;
        this.hitChance = hitChance;
        this.fireDamage = fireDamage;
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
