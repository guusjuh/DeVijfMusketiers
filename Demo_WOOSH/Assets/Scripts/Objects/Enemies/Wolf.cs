using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 30;
        this.totalActionPoints = 2;

        base.Initialize(startPos);
    }
}
