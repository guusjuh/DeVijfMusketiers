using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerTileManager {
    private const int EXTRA_ROWS = 5;
    private const int EXTRA_COLS = 7;

    private const float Y_OFFSET = -2.55f;

    private int rows;
    private int columns;
    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    public int LowerToHigherCoordX(int x) { return x - EXTRA_ROWS; }
    public int LowerToHigherCoordY(int y) { return y - EXTRA_COLS; }
    public Coordinate LowerToHigherCoord(Coordinate coord) { return new Coordinate(LowerToHigherCoordX(coord.x), LowerToHigherCoordY(coord.y)); }

    public int HigherToLowerCoordX(int x) { return x + EXTRA_ROWS; }
    public int HigherToLowerCoordY(int y) { return y + EXTRA_COLS; }
    public Coordinate HigherToLowerCoord(Coordinate coord) { return new Coordinate(HigherToLowerCoordX(coord.x), HigherToLowerCoordY(coord.y)); }

    private bool HigherGridOccupied(Coordinate coord)
    {
        bool outsideHigher = coord.x < EXTRA_ROWS || 
                             coord.x >= tileManagerRef.Rows + EXTRA_ROWS ||
                             coord.y < EXTRA_COLS || 
                             coord.y >= tileManagerRef.Columns + EXTRA_COLS;

        return outsideHigher ? false : tileManagerRef.GetNodeReference(LowerToHigherCoord(coord)).GetSecType() != SecTileType.Gap;
    }

    public bool NoContent(Coordinate coord)
    {
        return (coord.x >= EXTRA_ROWS - 1 && coord.x <= tileManagerRef.Rows + EXTRA_ROWS
            && coord.y >= EXTRA_COLS && coord.y < tileManagerRef.Columns + EXTRA_COLS + 1);
    }

    private LowerTileNode[,] grid;
    private GameObject gridParent;
    public GameObject GridParent { get { return gridParent; } }

    private TileManager tileManagerRef;

    public void Initialize()
    {
        tileManagerRef = GameManager.Instance.TileManager;

        gridParent = new GameObject("Grid Parent");
        gridParent.transform.SetParent(tileManagerRef.GridParent.transform);
        gridParent.transform.position = Vector3.zero;

        if (!UberManager.Instance.DevelopersMode) SetUpGrid();
    }

    public void Restart()
    {
        if (!UberManager.Instance.DevelopersMode) SetUpGrid();
    }

    private void SetUpGrid()
    {
        // Get the amount of rows and colomns from the level
        rows = (2 * EXTRA_ROWS) + ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).rows;
        columns = (2 * EXTRA_COLS) + ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).columns;

        // Initialize the grid 2D array.
        grid = new LowerTileNode[rows, columns];

        // Create the grid.
        CreateGrid();

        // Let every node find it's neighbours.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j].SearchNeighbours();
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == null) continue;
                grid[i, j].Clear();
            }
        }

        grid = null;
    }

    private void CreateGrid()
    {
        EnvironmentType environmentType = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).environmentType;

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Determine grid- and worldposition. 
                Coordinate gridPosition = new Coordinate(i, j);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                bool higherGridOccupied = HigherGridOccupied(gridPosition);

                // Create the grid node. 
                LowerTileNode tileNode = new LowerTileNode(gridPosition, worldPosition, higherGridOccupied);
                tileNode.Hexagon.transform.parent = gridParent.transform;

                // Add the grid node to the grid array. 
                grid[i, j] = tileNode;
            }
        }
    }

    public Vector2 GetWorldPosition(Vector2 gridPosition)
    {
        // Calculate gridpoint world position.
        int higherX = LowerToHigherCoordX((int)gridPosition.x);
        int higherY = LowerToHigherCoordY((int)gridPosition.y);

        float xPos = higherX * tileManagerRef.HexagonWidth;
        float yPos = higherY * (tileManagerRef.HexagonHeight * 2) 
            + (tileManagerRef.HexagonHeight * (Mathf.Abs(higherX % 2)))
            + (tileManagerRef.HexagonHeight * Y_OFFSET);

        // Return world position.
        return new Vector2(xPos, yPos);
    }

    /// <summary>
    /// Calculate world position from grid position.
    /// </summary>
    /// <param name="gridPosition">The position of the node in the grid.</param>
    /// <returns></returns>
    public Vector2 GetWorldPosition(Coordinate gridPosition)
    {
        // Calculate gridpoint world position.
        int higherX = LowerToHigherCoordX(gridPosition.x);
        int higherY = LowerToHigherCoordY(gridPosition.y);

        float xPos = higherX * tileManagerRef.HexagonWidth;
        float yPos = higherY * (tileManagerRef.HexagonHeight * 2) 
            + (tileManagerRef.HexagonHeight * (Mathf.Abs(higherX % 2))) 
            + (tileManagerRef.HexagonHeight * Y_OFFSET);

        // Return world position.
        return new Vector2(xPos, yPos);
    }

    public Coordinate GetGridPosition(Vector2 worldPosition)
    {
        // Return if the grid is empty.
        if (grid.Length < 1) return Coordinate.zero;

        // Get the distance from the first node to the given worldposition.
        float distance = Vector3.Distance(GetWorldPosition(new Coordinate(0, 0)), worldPosition);

        // Set the node closest to the given worldposition.
        Coordinate closeCoord = new Coordinate(0, 0);

        // Check for every node.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // If the node is closer to the given world position than the current closed distance.
                if (distance > Vector3.Distance(GetWorldPosition(new Coordinate(i, j)), worldPosition))
                {
                    // Set the node as new closest node.
                    distance = Vector3.Distance(GetWorldPosition(new Coordinate(i, j)), worldPosition);
                    closeCoord = new Coordinate(i, j);
                }
            }
        }

        return closeCoord;
    }

    /// <summary>
    /// Get a reference to the grid node.
    /// </summary>
    /// <param name="gridPos">The position of the grid node.</param>
    /// <returns></returns>
    public LowerTileNode GetNodeReference(Coordinate gridPos)
    {
        // Check for every grid node.
        foreach (LowerTileNode t in grid)
        {
            // If the grid node is located at the given grid position, return it.
            if (t != null && t.GridPosition == gridPos)
                return t;
        }

        // No matching node is found. 
        return null;
    }
}
