using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode {
    /// <summary>
    /// The position of the node in the grid.
    /// </summary>    
    private Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    /// <summary>
    /// The position of the node in the world.
    /// </summary>    
    private Vector3 worldPosition;
    public Vector3 WorldPosition { get { return worldPosition; } }

    /// <summary>
    /// List of the neighbours of the grid node. 
    /// </summary>
    private List<TileNode> neighbours;
    public List<TileNode> NeightBours { get { return neighbours; } }
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
    /// The content on this tile.
    /// </summary>
    private TileContent content;
    public TileContent Content { get { return content;} }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="gridPosition">The grid position.</param>
    /// <param name="worldPosition">The world position.</param>
    public TileNode(Coordinate gridPosition, Vector3 worldPosition)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;

        neighbours = new List<TileNode>();

        content = new TileContent();

        // Create a hexagon of the given type and store a reference.
        hexagon = CreateHexagon();
    }

    public void SetTestColor(bool on, Color color = default(Color))
    {
        hexagon.GetComponent<SpriteRenderer>().color = on ? color : Color.white;
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
        // Check for neighbours in every possible direction. 
        for (int i = 0; i < GameManager.Instance.TileManager.Directions(gridPosition).Length; i++)
        {
            // The position of the searched neighbour
            Coordinate tempPos = gridPosition + GameManager.Instance.TileManager.Directions(gridPosition)[i];

            // If this position is greater than the minimum of the grid.And if this position is smaller than the maximum of the grid.
            if (tempPos.x >= 0 && tempPos.y >= 0 && tempPos.x < GameManager.Instance.TileManager.Columns &&
                tempPos.y < GameManager.Instance.TileManager.Rows)
            {
                // Check for the position containing a node and add it to the list of neighbours.
                if (GameManager.Instance.TileManager.Grid[tempPos.x, tempPos.y] != null)
                    neighbours.Add(GameManager.Instance.TileManager.Grid[tempPos.x, tempPos.y]);
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

    /// <summary>
    /// Calculate distance from a position
    /// </summary>
    /// <param name="position">The position of which the distance to this node will be calculated.</param>
    /// <returns></returns>
    public float DistanceFromPosition(TileNode node)
    {
        return (node.WorldPosition - worldPosition).sqrMagnitude;
    }

    public int EnterCost()
    {
        return content.EnterCost();
    }
}
