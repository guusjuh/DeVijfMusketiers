using System.Collections.Generic;
using UnityEngine;

public class EnemyBrokenGround : Action
{

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        cost = 1;
    }

    public override bool DoAction()
    {
        float distance = (parent.GridPosition.EuclideanDistance(parent.target.GridPosition));
        float maxDistance = GameManager.Instance.TileManager.FromTileToTile;
        bool closeEnough = distance - maxDistance <= 0.01f;
        bool enoughAP = parent.CurrentActionPoints >= cost;

        if (closeEnough && enoughAP)
        {
            Vector2 position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            Coordinate tempCoord = GameManager.Instance.TileManager.GetGridPosition(position);
            TileNode tile = GameManager.Instance.TileManager.GetNodeReference(tempCoord);
            List<TileNode> nodes = tile.NeightBours;
            foreach (var node in nodes)
            {
                node.Content.KillTileContent();
            }
            parent.EndMove(cost);

            return true;
        }
        return false;
    }
}
