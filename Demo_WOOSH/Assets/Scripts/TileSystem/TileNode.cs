using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode {
    /// <summary>
    /// The position of the node in the grid.
    /// </summary>    
    private Vector2 gridPosition;
    public Vector2 GridPosition { get { return gridPosition; } }

    /// <summary>
    /// The position of the node in the world.
    /// </summary>    
    private Vector3 worldPosition;

    /// <summary>
    /// List of the neighbours of the grid node. 
    /// </summary>
    private List<TileNode> neighbours;

    /// <summary>
    /// The game object of the node. 
    /// </summary>
    private GameObject hexagon;
    public GameObject Hexagon { get { return hexagon; } }

    /// <summary>
    /// True after node is completely instantiated. 
    /// </summary>
    private bool instantiated;
    public bool Instantiated { get { return instantiated; } }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="_gridPosition">The position in the grid.</param>
    /// <param name="_worldPosition">The position in the world.</param>
    /// <param name="hexagonType">The type of hexagon.</param>
    public TileNode(Vector2 gridPosition, Vector3 worldPosition)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;

        neighbours = new List<TileNode>();

        // Create a hexagon of the given type and store a reference.
        hexagon = CreateHexagon();
    }

    public void Clear()
    {
        neighbours.Clear();
        neighbours = null;

        GameObject.Destroy(hexagon);
    }

    /// <summary>
    /// Create a hexagon gameobject.
    /// </summary>
    /// <param name="hexagonType">The type of the hexagon.</param>
    /// <returns></returns>
    private GameObject CreateHexagon()
    {
        Object prefab = Resources.Load<GameObject>("Prefabs/Hexagons/Hexagon");

        GameObject hexagonObject = GameObject.Instantiate(prefab) as GameObject;
        hexagonObject.name = prefab.name;

        // Set the position.
        hexagonObject.transform.position = worldPosition;

        // Return the created hexagon.
        return hexagonObject;
    }

    /// <summary>
    /// Search for neighbour nodes. 
    /// </summary>
    public void SearchNeighbours()
    {
        // Declare the directions in which a neighbour can be.
        Vector2[] directions = new Vector2[] {
            new Vector2(0, 1), new Vector2(1, -1), new Vector2(1, 0),
            new Vector2(0, -1), new Vector2(-1, 1), new Vector2(-1, 0)
        };

        // Check for neighbours in every possible direction. 
        for (int i = 0; i < directions.Length; i++)
        {
            // The position of the searched neighbour
            Vector2 tempPos = gridPosition + directions[i];

            // If this position is greater than the minimum of the grid.
            if (tempPos.x >= 0 && tempPos.y >= 0)
            {
                // And if this position is smaller than the maximum of the grid.
                if (tempPos.x < GameManager.Instance.TileManager.Columns && tempPos.y < GameManager.Instance.TileManager.Rows)
                {
                    // Check for the position containing a node and add it to the list of neighbours.
                    if (GameManager.Instance.TileManager.Grid[(int)tempPos.x, (int)tempPos.y] != null) neighbours.Add(GameManager.Instance.TileManager.Grid[(int)tempPos.x, (int)tempPos.y]);
                }
            }
        }
    }

    /// <summary>
    /// Calculate distance from a position
    /// </summary>
    /// <param name="position">The position of which the distance to this node will be calculated.</param>
    /// <returns></returns>
    public float DistanceFromPosition(Vector3 position)
    {
        return (position - worldPosition).sqrMagnitude;
    }
}
