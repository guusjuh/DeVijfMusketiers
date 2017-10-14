using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager
{
    public enum TileType
    {
        Normal = 0,
        Goo,
    }

    public enum ContentType
    {
        Unknown = -1,
        Human = 0,
        InivisbleHuman,
        Barrel,
        BrokenBarrel,
        Shrine,
        FlyingMonster,
        WalkingMonster,
    }

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

    private static Color PATHCOLOR = Color.white;//new Color(1.0f, 0.95f, 0.6f, 1.0f);
    private static Color TARGETCOLOR = new Color(0.99f, 0.02f, 0.02f, 1.0f);

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
    private float hexagonScale = 1.2f;
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

    private List<TileNode> highlightedNodes;

    /// <summary>
    /// Initialize the gridsystem
    /// </summary>
    public void Initialize()
    {
        gridParent = new GameObject("Grid Parent");

        SetUpGrid();
    }

    public void Restart()
    {
        SetUpGrid();
    }

    private void SetUpGrid()
    {
        // Get the amount of rows and colomns from the level
        this.rows = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].rows;
        this.columns = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].columns;

        // Initialize the grid 2D array.
        grid = new TileNode[rows, columns];

        // Initialize and set the parent. 

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

    public void ClearGrid()
    {
        HidePossibleRoads();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
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
    private void CreateGrid(int sizeX, int sizeY)
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                // Determine grid- and worldposition. 
                Coordinate gridPosition = new Coordinate(i, j);
                Vector3 worldPosition = GetWorldPosition(gridPosition);

                // Create the grid node. 
                TileNode tileNode = new TileNode(gridPosition, worldPosition);
                tileNode.Hexagon.transform.parent = gridParent.transform;

                // Add the grid node to the grid array. 
                grid[i, j] = tileNode;
            }
        }
    }


    public List<TileNode> GeneratePathTo(Coordinate from, Coordinate to, ContentType type)
    {
        //if (!UnitCanEnterTile(toX, toY))
        //    return null;

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

            /*if (u == target)
                break; //exit while loop if target is found*/

            unvisited.Remove(u);

            foreach (TileNode v in u.NeightBours)
            {
                //float alt = dist[u] + u.DistTo(v);
                float alt = dist[u] + CostToEnterTile(v, target, type);

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

    public float CostToEnterTile(TileNode nextTile, ContentType type)
    {
        //if (!UnitCanEnterTile(nextTile.GridPosition, type))
        //    return nextTile.EnterCost() + 1; 

        return nextTile.EnterCost();
    }

    public float CostToEnterTile(TileNode nextTile, TileNode endTile, ContentType type)
    {
        if (new Vector2(endTile.GridPosition.x - nextTile.GridPosition.x, endTile.GridPosition.y - nextTile.GridPosition.y).magnitude > 0.1f)
        {
            if (!UnitCanEnterTile(nextTile.GridPosition, type))
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
    /// //TODO: implement flying behavior
    public bool UnitCanEnterTile(Coordinate pos, ContentType type)
    {
        if (pos.x < 0 || pos.x >= rows || pos.y < 0 || pos.y >= columns)
        {
            return false;
        }

        List<ContentType> typesToEnter = GameManager.Instance.TypesToEnter.Get(type);

        for (int j = 0; j < grid[pos.x, pos.y].Content.ContentTypes.Count; j++)
        {
            if (!typesToEnter.Contains(grid[pos.x, pos.y].Content.ContentTypes[j]))
            {
                return false;
            }
        }

        return true;
    }

    public void SetTileType(TileManager.TileType type, Coordinate pos)
    {
        grid[pos.x,pos.y].Content.SetTileType(type);
    }

    public void SetObject(Coordinate pos, ContentType type)
    {
        grid[pos.x, pos.y].Content.AddContent(type);
    }

    public void RemoveObject(Coordinate pos, ContentType type)
    {
        grid[pos.x, pos.y].Content.RemoveContent(type);
    }

    public void MoveObject(Coordinate currPos, Coordinate nextPos, ContentType type)
    {
        RemoveObject(currPos, type);
        SetObject(nextPos, type);
    }

    public void SwitchStateTile(TileManager.ContentType type, Coordinate pos)
    {
        TileManager.ContentType newContentType = TileManager.ContentType.Unknown;
        switch (type)
        {
            case ContentType.Barrel:
                newContentType = ContentType.BrokenBarrel;
                break;
            case ContentType.BrokenBarrel:
                newContentType = ContentType.Barrel;
                break;
            case ContentType.Human:
                newContentType = ContentType.InivisbleHuman;
                break;
            case ContentType.InivisbleHuman:
                newContentType = ContentType.Human;
                break;
        }

        grid[pos.x, pos.y].Content.ReplaceContent(type, newContentType);
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
            if (t.GridPosition == gridPos)
                return t;
        }

        // No matching node is found. 
        return null;
    }

    private List<TileNode> GetNodeWithGooReferences()
    {
        List<TileNode> gooNodes = new List<TileNode>();

        foreach (TileNode t in grid)
        {
            if(t.Content.TileType == TileType.Goo)
                gooNodes.Add(t);
        }

        return gooNodes;
    }

    public List<TileNode> GetPossibleGooNodeReferences()
    {
        List<TileNode> gooNodes = GetNodeWithGooReferences();
        List<TileNode> possGooNodes = new List<TileNode>();

        for (int i = 0; i < gooNodes.Count; i++)
        {
            for (int j = 0; j < Directions(gooNodes[i].GridPosition).Length; j++)
            {
                Coordinate currPos = gooNodes[i].GridPosition + Directions(gooNodes[i].GridPosition)[j];

                if (gooNodes.Contains(GetNodeReference(currPos)) ||
                    currPos.x < 0 || currPos.x >= rows ||
                    currPos.y < 0 || currPos.y >= columns)
                {
                    continue;
                }
                else
                {
                    //check all neighbours and add this positions for each neighbor which is a goo tile.
                    for (int k = 0; k < Directions(currPos).Length; k++)
                    {
                        Coordinate neighbourPos = Directions(currPos)[k] + currPos;
                        if (gooNodes.Contains(GetNodeReference(neighbourPos)))
                        {
                            possGooNodes.Add(GetNodeReference(currPos));
                        }
                    }
                }
            }
        }

        return possGooNodes;
    }

    public void ShowPossibleRoads(Coordinate gridPos, int actionPoints)
    {
        highlightedNodes = new List<TileNode>();

        // add yourself
        TileNode hisNode = GetNodeReference(gridPos);
        highlightedNodes.Add(hisNode);

        RecursiveTileFinder(highlightedNodes, hisNode, actionPoints, gridPos);

        // highlight all found buttons
        highlightedNodes.HandleAction(n =>
        {
            if (n.Content.ContentTypes.Count == 0 || n.Content.ContentTypes.Contains(ContentType.WalkingMonster) 
            || n.Content.ContentTypes.Contains(ContentType.BrokenBarrel) || n.Content.ContentTypes.Contains(ContentType.InivisbleHuman))
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
    public void ShowExtraTargetForSpecial(Coordinate gridPos, int maxDistance)
    {
        List<TileNode> nodes = new List<TileNode>();
        TileNode hisNode = GetNodeReference(gridPos);
        RecursiveTileFinder(nodes, hisNode, maxDistance, gridPos);

        // highlight all found humans
        nodes.HandleAction(n =>
        {
            if (n.Content.ContentTypes.Contains(ContentType.Human))
            {
                highlightedNodes.Add(n);
                n.HighlightTile(true, TARGETCOLOR);
            }
        });
    }

    private void RecursiveTileFinder(List<TileNode> nodes, TileNode thisNode, int distance, Coordinate startPos, bool usingCost = true)
    {
        //TODO: remove actionpoints
        if (distance >= -1)
        {
            nodes.Add(thisNode);

            if (distance > 0)
            {
                for (int i = 0; i < thisNode.NeightBours.Count; i++)
                {
                    float lastDist = thisNode.GridPosition.ManhattanDistance(startPos);
                    float currDist = thisNode.NeightBours[i].GridPosition.ManhattanDistance(startPos);

                    if (currDist >= lastDist)
                    {
                        //TODO: this should be a more generic monster type
                        RecursiveTileFinder(nodes, 
                                            thisNode.NeightBours[i],
                                            distance - (usingCost ? (int)CostToEnterTile(thisNode.NeightBours[i], ContentType.WalkingMonster) : 1), 
                                            startPos);
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
}
