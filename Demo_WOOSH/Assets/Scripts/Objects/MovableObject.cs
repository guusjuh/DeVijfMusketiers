using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : EnemyTarget {

    public void Teleport(Coordinate newPos)
    {
        GameManager.Instance.TileManager.MoveObject(gridPosition, newPos, this);

        gridPosition = newPos;
        Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(gridPosition);

        UIManager.Instance.InGameUI.ActivateTeleportButtons(false);

        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    public bool CanBePushed()
    {
        for (int i = 0; i < GameManager.Instance.TileManager.Directions(gridPosition).Length; i++)
        {
            if (GameManager.Instance.TileManager.GetNodeReference(gridPosition + GameManager.Instance.TileManager.Directions(gridPosition)[i]) != null)
            {
                if (GameManager.Instance.TileManager.GetNodeReference(gridPosition + GameManager.Instance.TileManager.Directions(gridPosition)[i]).Content.CompletelyEmpty())
                {
                    return true;
                }
            }
        }
        return false;
    }
}
