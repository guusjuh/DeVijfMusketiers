using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportComponent : SpellComponent {
    public TeleportComponent(int cost, int range)
    {
        this.cost = cost;
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
            UberManager.Instance.InputManager.highlightsActivated = true;
            //-listen for click on certain tile
            //-activate execute method
            return true;
        }

        return false;
    }

    public override bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        target.Teleport(UberManager.Instance.SpellManager.SelectedTile);
        UberManager.Instance.GameManager.TileManager.DisableHighlights();
        base.Execute(target, rnd, endTurn);
        return true;
    }
}
