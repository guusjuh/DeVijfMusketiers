using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : EnemyTarget {

    public void Push(Coordinate dirToGo)
    {
        GameManager.Instance.TileManager.MoveObject(gridPosition, gridPosition + dirToGo, type);

        gridPosition += dirToGo;
        Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(gridPosition);

        GameManager.Instance.UiManager.ActivatePushButtons(false, this);

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
