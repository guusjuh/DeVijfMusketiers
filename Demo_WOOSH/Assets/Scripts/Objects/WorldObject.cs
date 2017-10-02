using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected TileManager.ContentType type;
    public TileManager.ContentType Type { get { return type; } }

    protected Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    public virtual void Initialize(Coordinate startPos)
    {
        gridPosition = startPos;
    }
}
