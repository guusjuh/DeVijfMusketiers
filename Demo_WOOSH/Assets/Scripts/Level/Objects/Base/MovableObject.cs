using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : EnemyTarget {

    public void Teleport(Coordinate newPos)
    {
        GameManager.Instance.TileManager.MoveObject(gridPosition, newPos, this);

        gridPosition = newPos;
        Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(gridPosition);

        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }
}
