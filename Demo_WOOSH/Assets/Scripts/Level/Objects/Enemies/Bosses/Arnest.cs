using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        startHealth = 100;
        hasSpecial = false;
        viewDistance = 3;

        type = SecContentType.Arnest;

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