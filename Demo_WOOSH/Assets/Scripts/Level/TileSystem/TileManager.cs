﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    private int debugCounter = 0;
    private static Coordinate[] directionsEven = new Coordinate[] {
            /*right up*/ new Coordinate(1, 0),
            /*left up*/ new Coordinate(-1, 0),

            /*up*/ new Coordinate(0, 1), 
            /*down*/ new Coordinate(0, -1), 

            /*right down*/ new Coordinate(1, -1), 
            /*left down*/ new Coordinate(-1, -1), 
        };

    private static Coordinate[] directionsUneven = new Coordinate[] {
            /*right up*/ new Coordinate(1, 1),
            /*left up*/ new Coordinate(-1, 1),

            /*up*/ new Coordinate(0, 1), 
            /*down*/ new Coordinate(0, -1), 

            /*right down*/ new Coordinate(1, 0), 
            /*left down*/ new Coordinate(-1, 0),
        };

    public Coordinate[] Directions(Coordinate from)
    {
        if (from.x % 2 == 0)
        {
            return directionsEven;
        }
        else
        {
            return directionsUneven;
        }
    }

    private static Color PATHCOLOR = Color.white;
    private static Color TARGETCOLOR = new Color(0.99f, 0.02f, 0.02f, 1.0f);

    private int rows;
    private int columns;
    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    private TileNode[,] grid;
    private GameObject gridParent;
    public GameObject GridParent { get { return gridParent; } }
    private List<TileNode> highlightedNodes;

    private Coordinate[] corners;
    public Coordinate[] Corners { get { return corners; } }

    // Hexagon scale.
    private float hexagonScale = 1.2f;
    public float HexagonScale { get { return hexagonScale; } }
    private float hexagonWidth = .75f;
    public float HexagonWidth { get { return hexagonWidth * hexagonScale; } }
    private float hexagonHeight = .433f;
    public float HexagonHeight { get { return hexagonHeight * hexagonScale; } }
    public float FromTileToTile { get { return (HexagonHeight * 2); } }

    public float FromTileToTileInCanvasSpace
    {
        get
        {
            Vector2 thisPos = UIManager.Instance.InGameUI.WorldToCanvas(GetWorldPosition(grid[0, 0].GridPosition));
            Vector2 otherPos = UIManager.Instance.InGameUI.WorldToCanvas(GetWorldPosition(grid[1, 0].GridPosition));

            return (thisPos - otherPos).magnitude;
        }
    }

    /// <summary>
    /// Initialize the gridsystem
    /// </summary>
    public void Initialize()
    {
        gridParent = new GameObject("Grid Parent");
        if (UberManager.Instance.DevelopersMode) SetUpEmptyGridDEVMODE();
        else SetUpGrid();

        corners = new Coordinate[4]
        {
            new Coordinate(     0,         0),
            new Coordinate(rows-1, columns-1),
            new Coordinate(rows-1,         0),
            new Coordinate(     0, columns-1)
        };
    }

    public void Restart()
    {
        if (UberManager.Instance.DevelopersMode) SetUpEmptyGridDEVMODE();
        else SetUpGrid();
    }

    private void SetUpGrid()
    {
        // Get the amount of rows and colomns from the level
        this.rows = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).rows;
        this.columns = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).columns;

        // Initialize the grid 2D array.
        grid = new TileNode[rows, columns];

        // Initialize and set the parent. 

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
        HidePossibleRoads();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if(grid[i,j] == null) continue;
                grid[i,j].Clear();
            }
        }

        grid = null;
    }

    /// <summary>
    /// Create the grid. 
    /// </summary>
    /// <param name="sizeX">Amount of hexagon rows.</param>
    /// <param name="sizeY">Amount of hexagon columns.</param>
    private void CreateGrid()
    {
        SecTileTypeRow[] tempGrid = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).grid;

        for (int i = 0; i < tempGrid.Length; i++)
        {
            for (int j = 0; j < tempGrid[0].row.Length; j++)
            {
                // Determine grid- and worldposition. 
                Coordinate gridPosition = new Coordinate(i, j);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                // Create the grid node. 
                TileNode tileNode = new TileNode(gridPosition, worldPosition, tempGrid[i].row[j]);
                tileNode.Hexagon.transform.parent = gridParent.transform;

                // Add the grid node to the grid array. 
                grid[i, j] = tileNode;
            }
        }
    }

    public List<TileNode> GeneratePathTo(Coordinate from, Coordinate to, WorldObject worldObject)
    {
        Dictionary<TileNode, float> dist = new Dictionary<TileNode, float>();
        Dictionary<TileNode, TileNode> prev = new Dictionary<TileNode, TileNode>();

        //setup 'Q', list of nodes that are not checked yet
        List<TileNode> unvisited = new List<TileNode>();

        TileNode source = grid[from.x, from.y];
        TileNode target = grid[to.x, to.y];

        dist[source] = 0;
        prev[source] = null;

        bool targetFound = false;

        //initialize everything to have infinity distance 
        foreach (TileNode v in grid)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            //u is unvisited node with smallest distance
            TileNode u = null;
            foreach (TileNode possible in unvisited)
            {
                if (u == null || dist[possible] < dist[u])
                    u = possible;
            }

            unvisited.Remove(u);

            foreach (TileNode v in u.NeightBours)
            {
                //float alt = dist[u] + u.DistTo(v);
                float alt = dist[u] + CostToEnterTile(v, target, worldObject);

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        //either found shortest route or there is no route at all
        if (prev[target] == null)
            return null; //no possible route 

        List<TileNode> currentPath = new List<TileNode>();

        TileNode curr = target;

        //loop through prev chain and add it to path
        while (curr != null)
        {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        //path is now route from target to source, so reverse
        currentPath.Reverse();

        return currentPath;
    }

    public float CostToEnterTile(TileNode nextTile, WorldObject worldObject)
    {
        bool isWalkingMonster = worldObject.IsMonster() && worldObject.IsWalking();

        if (nextTile.GetType() == TileType.Dangerous && isWalkingMonster)
            return 1000;

        return nextTile.EnterCost();
    }

    public float CostToEnterTile(TileNode nextTile, TileNode endTile, WorldObject worldObject)
    {
        if (new Vector2(endTile.GridPosition.x - nextTile.GridPosition.x, endTile.GridPosition.y - nextTile.GridPosition.y).magnitude > 0.1f)
        {
            if (!UnitCanEnterTile(nextTile.GridPosition, worldObject))
                return Mathf.Infinity;
        }

        float cost = nextTile.EnterCost();

        return cost;
    }

    /// <summary>
    /// Units the can enter tile (ENEMIES ONLY)
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public bool UnitCanEnterTile(Coordinate pos, WorldObject worldObject)
    {
        if (pos.x < 0 || pos.x >= rows || pos.y < 0 || pos.y >= columns)
        {
            return false;
        }

        bool isWalkingMonster = worldObject.IsMonster() && worldObject.IsWalking();

        if (grid[pos.x, pos.y].GetType() == TileType.Dangerous && isWalkingMonster)
        {
            return false;
        }

        bool canEnterBarrels = worldObject.IsMonster();
        for (int i = 0; i < grid[pos.x, pos.y].GetAmountOfContent(); i++)
        {
            if (grid[pos.x, pos.y].GetContent()[i].IsBarrel() && canEnterBarrels) continue;
            else return false;
        }

        return true;
    }

    public void SetObject(Coordinate pos, WorldObject worldObject)
    {
        if (GetNodeReference(pos) == null) return;

        grid[pos.x, pos.y].AddContent(worldObject);
    }

    public void RemoveObject(Coordinate pos, WorldObject worldObject)
    {
        if (GetNodeReference(pos) == null) return;

        grid[pos.x, pos.y].RemoveContent(worldObject);
    }

    public void MoveObject(Coordinate currPos, Coordinate nextPos, WorldObject worldObject)
    {
        if (GetNodeReference(currPos) == null) return;
        if (GetNodeReference(nextPos) == null) return;

        RemoveObject(currPos, worldObject);
        SetObject(nextPos, worldObject);
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
    /// Calculate world position from grid position.
    /// </summary>
    /// <param name="gridPosition">The position of the node in the grid.</param>
    /// <returns></returns>
    public Vector2 GetWorldPosition(Coordinate gridPosition)
    {
        // Calculate gridpoint world position.
        float xPos = gridPosition.x * HexagonWidth;
        float yPos = gridPosition.y * (HexagonHeight * 2) + (HexagonHeight * (Mathf.Abs(gridPosition.x % 2)));

        // Return world position.
        return new Vector2(xPos, yPos);
    }

    public Coordinate GetGridPosition(Vector2 worldPosition)
    {
        // Return if the grid is empty.
        if (grid.Length < 1) return Coordinate.zero;

        // Get the distance from the first node to the given worldposition.
        float distance = Vector3.Distance(GetWorldPosition(new Coordinate(0,0)), worldPosition);

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
    public TileNode GetNodeReference(Coordinate gridPos)
    {
        // Check for every grid node.
        foreach (TileNode t in grid)
        {
            // If the grid node is located at the given grid position, return it.
            if (t != null && t.GridPosition == gridPos)
                return t;
        }

        // No matching node is found. 
        return null;
    }

    public List<TileNode> GetNodeWithGapReferences()
    {
        List<TileNode> gapNodes = new List<TileNode>();

        foreach (TileNode t in grid)
        {
            if (t != null && t.GetSecType() == SecTileType.Gap)
                gapNodes.Add(t);
        }

        return gapNodes;
    }

    public TileNode GetPossibleGapNodeReferences()
    {
        List<TileNode> gapNodes = GetNodeWithGapReferences();
        Dictionary<TileNode, int> possGapNodes = new Dictionary<TileNode, int>();

        // find and add all possible gap nodes
        for (int i = 0; i < gapNodes.Count; i++)
        {
            for (int j = 0; j < Directions(gapNodes[i].GridPosition).Length; j++)
            {
                Coordinate currPos = gapNodes[i].GridPosition + Directions(gapNodes[i].GridPosition)[j];

                if (gapNodes.Contains(GetNodeReference(currPos)) ||
                    currPos.x < 0 || currPos.x >= rows ||
                    currPos.y < 0 || currPos.y >= columns ||
                    possGapNodes.ContainsKey(GetNodeReference(currPos)) ||
                    GetNodeReference(currPos).ContainsMonster())
                {
                    continue;
                }
                else
                {
                    possGapNodes.Add(GetNodeReference(currPos), 
                                     GetNodeReference(currPos).NeightBours.FindAll(
                                         n => n.GetSecType() == SecTileType.Gap).Count);
                }
            }
        }

        return UberManager.PerformRandomRoll<TileNode>(possGapNodes);
    }

    public void ShowPossibleRoads(WorldObject worldObject, Coordinate gridPos, int actionPoints)
    {
        highlightedNodes = new List<TileNode>();

        // add yourself
        TileNode hisNode = GetNodeReference(gridPos);

        RecursiveTileFinder(worldObject, highlightedNodes, hisNode, actionPoints, gridPos);

        // highlight all found buttons
        highlightedNodes.HandleAction(n =>
        {
            if (n.GetContent().Count == 0 || n.ContainsWalkingMonster() || n.ContainsBrokenBarrel())
            {
                n.HighlightTile(true, PATHCOLOR);
            }
            else
            {
                n.HighlightTile(true, TARGETCOLOR);
            }
        });
    }

    // assumed this is always called AFTER ShowPossibleRoads, no new list need to be set and the tiles can just be added to the highlighted
    public void ShowExtraTargetForSpecial(WorldObject worldObject, Coordinate gridPos, int maxDistance)
    {
        List<TileNode> nodes = new List<TileNode>();
        TileNode hisNode = GetNodeReference(gridPos);
        RecursiveTileFinder(worldObject, nodes, hisNode, maxDistance, gridPos, false);

        // highlight all found humans
        nodes.HandleAction(n =>
        {
            if (n.ContainsHuman())
            {
                highlightedNodes.Add(n);
                n.HighlightTile(true, TARGETCOLOR);
            }
        });
    }

    private void RecursiveTileFinder(WorldObject worldObject, List<TileNode> nodes, TileNode thisNode, int distance, Coordinate startPos, bool usingCost = true)
    {
        if (distance >= -1)
        {
            nodes.Add(thisNode);

            if (distance > 0)
            {
                for (int i = 0; i < thisNode.NeightBours.Count; i++)
                {
                    float lastDist = thisNode.GridPosition.EuclideanDistance(startPos);
                    float currDist = thisNode.NeightBours[i].GridPosition.EuclideanDistance(startPos);

                    if (currDist >= lastDist)
                    {
                        RecursiveTileFinder(worldObject, 
                                            nodes, 
                                            thisNode.NeightBours[i],
                                            distance - (usingCost ? (int)CostToEnterTile(thisNode.NeightBours[i], worldObject) : 1), 
                                            startPos, 
                                            usingCost);
                    }
                }
            }
        }
    }

    public void HidePossibleRoads()
    {
        if (highlightedNodes != null && highlightedNodes.Count != 0)
        {
            highlightedNodes.HandleAction(n => n.HighlightTile(false));
            highlightedNodes.Clear();
            highlightedNodes = null;
        }
    }

    public bool InRange(int viewDist, WorldObject obj1, WorldObject obj2)
    {
        // calculate distance
        float distance = obj1.GridPosition.EuclideanDistance(obj2.GridPosition);

        if (distance <= (viewDist * FromTileToTile)) return true;
        else return false;
    }

    public bool InRange(int viewDist, Coordinate coord1, Coordinate coord2)
    {
        // calculate distance
        float distance = coord1.EuclideanDistance(coord2);

        if (distance <= (viewDist * FromTileToTile)) return true;
        else return false;
    }


    // ---------------------------- DEVMODE FUNCTIONS -------------------------------------------

    private void SetUpEmptyGridDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return;

        rows = UberManager.Instance.LevelEditor.Rows;
        columns = UberManager.Instance.LevelEditor.Columns;

        grid = new TileNode[rows, columns];
    }

    public void AdjustGridSizeDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return;

        int oldRows = rows;
        int oldColumns = columns;

        rows = UberManager.Instance.LevelEditor.Rows;
        columns = UberManager.Instance.LevelEditor.Columns;

        TileNode[,] tempGrid = grid;

        grid = new TileNode[rows, columns];

        PasteGridDEVMODE(tempGrid, grid);

        ClearGridDEVMODE(tempGrid);

        tempGrid = null;
    }

    private void PasteGridDEVMODE(TileNode[,] oldGrid, TileNode[,] newGrid)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        for (int i = 0; i < oldGrid.GetLength(0); i++)
        {
            for (int j = 0; j < oldGrid.GetLength(1); j++)
            {
                // if it doesn't fit in the new grid, continue
                if (i >= newGrid.GetLength(0) || j >= newGrid.GetLength(1)) continue;

                // if the node was null, continue
                if (oldGrid[i, j] == null) continue;

                // Add the grid node to the grid array. 
                newGrid[i, j] = oldGrid[i, j];

                oldGrid[i, j] = null;
            }
        }
    }

    public void SetTileTypeDEVMODE(SecTileType type, Coordinate pos)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        if (GetNodeReference(pos) == null)
        {
            grid[pos.x, pos.y] = new TileNode(pos, GetWorldPosition(pos), type);
        }
        else
        {
            grid[pos.x, pos.y].CreateHexagon(type);
        }
    }

    public void RemoveTileDEVMODE(Coordinate pos)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        if (GetNodeReference(pos) != null)
        {
            RemoveContentDEVMODE(GetNodeReference(pos));

            GetNodeReference(pos).Clear();
            grid[pos.x, pos.y] = null;
        }
    }

    public void RemoveContentDEVMODE(TileNode node)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        if (node != null &&
            node.GetAmountOfContent() > 0)
        {
            WorldObject removedObject = node.RemoveContent();
            GameManager.Instance.LevelManager.RemoveObject(removedObject);
        }
    }

    public void ClearGridDEVMODE(TileNode[,] gridToRemove = null)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        if (gridToRemove == null) gridToRemove = grid;

        HidePossibleRoads();

        for (int i = 0; i < gridToRemove.GetLength(0); i++)
        {
            for (int j = 0; j < gridToRemove.GetLength(1); j++)
            {
                if (gridToRemove[i, j] == null) continue;

                if (gridToRemove[i, j].GetAmountOfContent() > 0)
                {
                    RemoveContentDEVMODE(gridToRemove[i, j]);
                }

                gridToRemove[i, j].Clear();
            }
        }

        gridToRemove = null;
    }

    public void FindNeighboursDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return;

        // Let every node find it's neighbours.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(grid[i, j] == null) continue;

                grid[i, j].SearchNeighbours();
            }
        }
    }

    public bool ValidGridDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return false;

        // Let every node find it's neighbours.
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] == null) return false;
            }
        }

        return true;
    }
}
