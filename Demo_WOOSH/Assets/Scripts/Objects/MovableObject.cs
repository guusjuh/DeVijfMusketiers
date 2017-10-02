using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : EnemyTarget {
    public bool CanBePushed()
    {
        for (int i = 0; i < GameManager.Instance.TileManager.Directions(gridPosition).Length; i++)
        {
            if (GameManager.Instance.TileManager.GetNodeReference(gridPosition).Content.CompletelyEmpty())
            {
                return true;
            }
        }

        return false;
    }
}
