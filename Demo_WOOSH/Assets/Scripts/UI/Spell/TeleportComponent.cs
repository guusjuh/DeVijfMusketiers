using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportComponent : SpellComponent {
    public TeleportComponent(int cost, SpellManager.SpellType type, int range)
    {
        this.cost = cost;
        this.type = type;
        isDirect = false;
        this.range = range;
    }
    
    public override bool Execute(WorldObject target, float rnd, bool endTurn)
    {
        target.Teleport(UberManager.Instance.SpellManager.SelectedTile);
        UberManager.Instance.GameManager.TileManager.DisableHighlights();
        UberManager.Instance.GameManager.LevelManager.Shrines.HandleAction(s => s.CheckForActive(true));

        base.Execute(target, rnd, endTurn);

        return true;
    }
}
