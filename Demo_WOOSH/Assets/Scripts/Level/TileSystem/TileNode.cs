using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode
{
    protected Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    protected Vector3 worldPosition;
    public Vector3 WorldPosition { get { return worldPosition; } }

    protected GameObject hexagon;
    public GameObject Hexagon { get { return hexagon; } }

    public TileNode(Coordinate gridPosition, Vector3 worldPosition)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
    }

    public virtual void Clear()
    {
        GameObject.Destroy(hexagon);
    }

    public virtual void SearchNeighbours() { }
}
