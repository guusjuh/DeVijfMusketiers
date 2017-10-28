using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        if(UberManager.Instance.Tutorial) this.startHealth = 10;
        else this.startHealth = 30;
        this.totalActionPoints = 2;

        this.hasSpecial = false;
        viewDistance = 3;

        this.type = SecContentType.Wolf;

        base.Initialize(startPos);
    }

    public override bool IsWalking() { return true; }
}
