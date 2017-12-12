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
        if (base.ApplyEffects(target, rnd))
        {
            //TODO: activate teleport
            //-highlight tiles
            UberManager.Instance.GameManager.TileManager.ShowTeleportHighlights(target, Range());
            //-listen for click on certain tile
            //-activate execute method
            return true;
        }

        return false;
    }
}
