using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private int rows;
    private int columns;
    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    /// <summary>
    /// The grid information. 
    /// </summary>
    private TileNode[,] grid;
    public TileNode[,] Grid { get { return grid; } }

    // Parent gameobject to make the hierarchy look cleaner. 
    private GameObject gridParent;

    // Hexagon scale.
    private float hexagonScale = 10f;
    public float HexagonScale { get { return hexagonScale; } }

    /// <summary>
    /// Hexagon width.
    /// </summary>
    private float hexagonWidth = .75f;
    public float HexagonWidth { get { return hexagonWidth * hexagonScale; } }

    /// <summary>
    /// Hexagon depth.
    /// </summary>
    private float hexagonHeight = .433f;
    private float HexagonHeight { get { return hexagonHeight * hexagonScale; } }

    /// <summary>
    /// Initialize the gridsystem
    /// </summary>
    public void Initialize(int rows, int columns)
    {
        // Get the amount of rows and colomns from the level
        this.rows = rows;
        this.columns = columns;
        
        // Initialize the grid 2D array.
        grid = new TileNode[rows, columns];

        // Initialize and set the parent. 
        gridParent = new GameObject("Grid Parent");

        // Create the grid.
        CreateGrid(rows, columns);

        // Let every node find it's neighbours.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j].SearchNeighbours();
            }
        }



    }

    /// <summary>
    /// Create the grid. 
    /// </summary>
    /// <param name="sizeX">Amount of hexagon rows.</param>
    /// <param name="sizeY">Amount of hexagon columns.</param>
    private void CreateGrid(int sizeX, int sizeY)
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                // Determine grid- and worldposition. 
                Vector2 gridPosition = new Vector2(i, j);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                // Create the grid node. 
                TileNode tileNode = new TileNode(gridPosition, worldPosition);
                tileNode.Hexagon.transform.parent = gridParent.transform;

                // Add the grid node to the grid array. 
                grid[i, j] = tileNode;
            }
        }
    }

    /// <summary>
    /// Calculate world position from grid position.
    /// </summary>
    /// <param name="gridPosition">The position of the node in the grid.</param>
    /// <returns></returns>
    public Vector2 GetWorldPosition(Vector2 gridPosition)
    {
        // Calculate gridpoint world position.
        float xPos = gridPosition.x * HexagonWidth;
        float yPos = gridPosition.y * (HexagonHeight * 2) + (HexagonHeight * (Mathf.Abs(gridPosition.x % 2)));

        // Return world position.
        return new Vector2(xPos, yPos);
    }

    /// <summary>
    /// Get node closest to given world position.
    /// </summary>
    /// <param name="position">The world position of the node.</param>
    /// <returns></returns>
    public TileNode GetTileNodeFromWorldPosition(Vector2 position)
    {
        // Return if the grid is empty.
        if (grid.Length < 1) return null;

        // Get the distance from the first node to the given worldposition.
        float distance = grid[0, 0].DistanceFromPosition(position);

        // Set the node closest to the given worldposition.
        TileNode closeNode = grid[0, 0];

        // Check for every node.
        for (int i = 1; i < grid.Length; i++)
        {
            for (int j = 1; j < grid.Length; j++)
            {
                // If the node is closer to the given world position than the current closed distance.
                if (distance > grid[i, j].DistanceFromPosition(position))
                {
                    // Set the node as new closest node.
                    distance = grid[i, j].DistanceFromPosition(position);
                    closeNode = grid[i, j];
                }
            }
        }

        // Return the node closest to the given worldposition.
        return closeNode;
    }

    /// <summary>
    /// Get a reference to the grid node.
    /// </summary>
    /// <param name="gridPos">The position of the grid node.</param>
    /// <returns></returns>
    public TileNode GetNodeReference(Vector2 gridPos)
    {
        // Check for every grid node.
        foreach (TileNode t in grid)
        {
            // If the grid node is located at the given grid position, return it.
            if (t.GridPosition == gridPos)
                return t;
        }

        // No matching node is found. 
        return null;
    }
}
