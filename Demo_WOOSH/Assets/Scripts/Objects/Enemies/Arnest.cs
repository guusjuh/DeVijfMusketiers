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

        base.Initialize(startPos);
    }

    protected override void Attack(EnemyTarget other)
    {
        if (other.Type == TileManager.ContentType.Human || other.Type == TileManager.ContentType.Shrine)
        {
            totalActionPoints++;
        }

        base.Attack(other);
    }
}