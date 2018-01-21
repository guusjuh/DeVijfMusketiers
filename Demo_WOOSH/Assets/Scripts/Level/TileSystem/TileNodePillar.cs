using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNodePillar : MonoBehaviour {
    private static Coordinate[] pillarDirectionsEven = new Coordinate[] {
            /*left down*/ new Coordinate(-1, -1),
            /*down*/ new Coordinate(0, -1), 
            /*right down*/ new Coordinate(1, -1)
        };

    private static Coordinate[] pillarDirectionsUneven = new Coordinate[] {
            /*left down*/ new Coordinate(-1, 0),
            /*down*/ new Coordinate(0, -1), 
            /*right down*/ new Coordinate(1, 0), 
        };

    public Coordinate[] PillarDirections(Coordinate from)
    {
        if (from.x % 2 == 0)
        {
            return pillarDirectionsEven;
        }
        else
        {
            return pillarDirectionsUneven;
        }
    }

    private List<SpriteRenderer> renderers;
    private LevelTileNode parent;

    public void Initialize(LevelTileNode parent)
    {
        this.parent = parent;

        this.transform.SetParent(parent.Hexagon.transform);
        //this.transform.localPosition = new Vector3(0.0f, -0.735f, 0.0f);

        renderers = new List<SpriteRenderer>();
        renderers.AddMultiple(gameObject.GetComponentsInChildren<SpriteRenderer>());

        renderers.HandleAction(r => r.gameObject.SetActive(false));

        CheckForActive();
    }

    public void CheckForActive()
    {
        if (parent.GetSecType() == SecTileType.Gap) return;

        Coordinate[] dirs = PillarDirections(parent.GridPosition);
        LevelTileNode currNode = null;

        for (int i = 0; i < dirs.Length; i++)
        {
            currNode = GameManager.Instance.TileManager.GetNodeReference(parent.GridPosition + dirs[i]);

            if (currNode == null || currNode.GetSecType() == SecTileType.Gap)
            {
                renderers[i].gameObject.SetActive(true);
            }
        }
    }

}
