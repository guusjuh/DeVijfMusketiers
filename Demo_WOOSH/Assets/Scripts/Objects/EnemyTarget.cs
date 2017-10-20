using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : WorldObject {

    protected bool canBeTargeted;
    public bool CanBeTargeted { get { return canBeTargeted; } }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        canBeTargeted = true;
    }

    public virtual bool Hit()
    {
        return false;
    }

    public virtual void DeadByGap()
    {
        canBeTargeted = false;
        GameManager.Instance.TileManager.RemoveObject(gridPosition, this);
        
        // in child: remove from lvlmanager 
    }
}
