using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportComponent : SpellComponent {
    public TeleportComponent(int range)
    {
        isDirect = false;
        this.range = range;
    }

    public override bool ApplyEffects(WorldObject target, float rnd)
    {
        //TODO: activate highlight buttons
        //-if range is 0, range is infinite

        return true;
    }
}
