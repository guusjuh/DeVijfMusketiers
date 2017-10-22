using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 100;
        this.hasSpecial = false;
        viewDistance = 3;

        this.type = SecContentType.Arnest;

        base.Initialize(startPos);
    }

    protected override void Attack(EnemyTarget other)
    {
        if (other.IsHuman() || other.IsShrine())
        {
            totalActionPoints++;
            calculatedTotalAP++;
        }

        base.Attack(other);
    }

    public override bool IsWalking()
    {
        return true;
    }
}