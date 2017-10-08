using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    int killedCount = 0;

    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 100;

        base.Initialize(startPos);
    }


    public override void StartTurn()
    {
        currentActionPoints = totalActionPoints + killedCount;

        if (spawnCooldown > 0)
        {
            spawnCooldown--;
        }

        SetUIInfo();
    }

    protected override void Attack(EnemyTarget other)
    {
        base.Attack(other);
        if (other.Type == TileManager.ContentType.Human || other.Type == TileManager.ContentType.Shrine)
        {
            killedCount++;
        }
    }
}