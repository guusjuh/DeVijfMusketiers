using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : BaseEnemy
{
    // initialize this instance
    public override void Initialize(int x, int y)
    {
        base.Initialize(x, y);

        totalActionPoints = 1;
        health = 10;

        // select the first target to destory! mwoehahaha
        target = SelectTarget();
        prevTarget = target;
        while (target.type == DamagableType.Human)
        {
            target.Targeted = false;
            target = SelectTarget();
            prevTarget = target;
        }

        currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        while (currentPath == null)
        {
            // there is no way the creature can get to that target right now.
            Debug.Log("Creature did not find a valid path");
            target.Targeted = false;

            target = SelectTarget();
            prevTarget = target;
            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        }

        if (currentPath.Count <= 2)
        {
            target.Targeted = true;
        }
    }

}
