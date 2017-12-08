
using System.Collections.Generic;
using UnityEngine;

public class EnemySelfDestruct : Action
{
    public override void StartTurn()
    {
        base.StartTurn();

        if(parent.HealthPercentage <= 10)
        {
            Vector2 position = new Vector2(parent.transform.position.x, parent.transform.position.y);
            Coordinate tempCoord = GameManager.Instance.TileManager.GetGridPosition(position);
            TileNode tile = GameManager.Instance.TileManager.GetNodeReference(tempCoord);
            List<TileNode> nodes = tile.NeightBours;
            foreach (var node in nodes)
            {
                node.Content.KillTileContent();
            }
            parent.EndMove(parent.CurrentActionPoints);
            parent.Kill();
        }
    }
}
