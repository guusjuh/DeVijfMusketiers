using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[Serializable]
public class TileMap
{
    public enum Types
    {
        Empty = 0,
        Human,
        InvisibleHooman,
        Barrel,
        BrokenBarrel,
        Shrine,
        Goo
    }

    [SerializeField]
    public TileType[] tileTypes;
    private Node[,] graph;
    private Types[,] tiles;
    public Types[,] Tiles { get { return tiles; } }

    private int columns, rows;

    public bool Empty(int x, int y, bool interstedInBats = true)
    {
        // outside board
        if (x < 0 || x >= columns || y < 0 || y >= rows)
        {
            return false;
        }

        // not empty tile
        if (interstedInBats)
        {
            if (tiles[x, y] != Types.Empty || graph[x, y].BatOnThisTile || graph[x,y].MonsterOnThisTile)
            {
                return false;
            }
        }
        else
        {
            if (tiles[x, y] != Types.Empty || graph[x,y].MonsterOnThisTile)
            {
                return false;
            }
        }

        return true;
    }

    public bool CanSpawnMinion(int x, int y)
    {
        // outside board
        if (x < 0 || x >= columns || y < 0 || y >= rows)
        {
            return false;
        }
        else
        {
            if (tiles[x, y] != Types.Barrel && tiles[x, y] != Types.Empty)
            {
                return false;
            }
        }

        return true;
    }

    public void Initialize(int columns, int rows, Types[,] generatedLevel)
    {
        this.columns = columns;
        this.rows = rows;

        GenerateMapData(generatedLevel);
        GeneratePathfindingGraph();
        //GenerateMapVisuals();
    }

    void GenerateMapData(Types[,] generatedLevel)
    {
        tiles = new Types[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                tiles[i, j] = generatedLevel[i, j];
            }
        }
    }

    void GeneratePathfindingGraph()
    {
        graph = new Node[columns, rows];

        //init node for each index in array
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                graph[x, y] = new Node();

                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        //calculate neightbour for each node
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //4-way connected map
                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < columns - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y < rows - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    void GenerateMapVisuals()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject go = GameObject.Instantiate(tileTypes[(int)tiles[x, y]].prefab, new Vector3(x, y, -5), Quaternion.identity) as GameObject;

                //ClickableTile ct = go.GetComponent<ClickableTile>();
                //ct.tileX = x;
                //ct.tileY = y;
                //ct.map = this;
            }
        }
    }

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY, Node endTarget, bool flying = false)
    {
        TileType tt = tileTypes[(int)tiles[targetX, targetY]];

        if (new Vector2(endTarget.x - targetX, endTarget.y - targetY).magnitude > 0.1f)
        {
            if (!UnitCanEnterTile(targetX, targetY, flying))
                return Mathf.Infinity;
        }

        float cost = flying ? 1 : tt.movementCost;

        //adding very tiny extra cost to diagonals, looks better
        if (sourceX != targetX && sourceY != targetY)
            cost += 0.001f;

        return cost;
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    public bool UnitCanEnterTile(int x, int y, bool flying = false)
    {
        //possible to check for walk/hover/fly skills or whatever against terrain types
        if (!flying)
        {
            if (tileTypes[(int) tiles[x, y]].canEnter)
            {
                return !graph[x, y].BatOnThisTile && !graph[x,y].MonsterOnThisTile;
            }

            return false;
        }
        else
        {
            if (tileTypes[(int) tiles[x, y]].canFlyOver)
            {
                return !graph[x, y].BatOnThisTile && !graph[x,y].MonsterOnThisTile;
            }

            return false;
        }
    }

    public void SetObject(int x, int y, Types type)
    {
        tiles[x, y] = type;
    }

    public void RemoveObject(int x, int y)
    {
        tiles[x, y] = Types.Empty;
    }

    public void MoveObject(int currX, int currY, int newX, int newY, Types type)
    {
        RemoveObject(currX, currY);
        SetObject(newX, newY, type);
    }

    public void SetBat(int x, int y)
    {
        graph[x, y].BatOnThisTile = true;
    }

    public void RemoveBat(int x, int y)
    {
        graph[x, y].BatOnThisTile = false;
    }

    public void MoveBat(int currX, int currY, int newX, int newY)
    {
        RemoveBat(currX, currY);
        SetBat(newX, newY);
    }

    public void SetMonster(int x, int y)
    {
        graph[x, y].MonsterOnThisTile = true;
    }

    public void RemoveMonster(int x, int y)
    {
        graph[x, y].MonsterOnThisTile = false;
    }

    public void MoveMonster(int currX, int currY, int newX, int newY)
    {
        RemoveMonster(currX, currY);
        SetMonster(newX, newY);
    }

    public void SwitchVaseStatus(int x, int y, bool broken)
    {
        // cannot walk on vases, not even when broken when commented out
        tiles[x, y] = broken ? Types.BrokenBarrel : Types.Barrel;
    }

    public void SwitchHoomanStatus(int x, int y, bool inktvis)
    {
        // cannot walk on vases, not even when broken when commented out
        tiles[x, y] = inktvis ? Types.InvisibleHooman : Types.Human;
    }

    public List<Node> GeneratePathTo(int fromX, int fromY, int toX, int toY, bool flying = false)
    {
        //if (!UnitCanEnterTile(toX, toY))
        //    return null;

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        //setup 'Q', list of nodes that are not checked yet
        List<Node> unvisited = new List<Node>();

        Node source = graph[fromX, fromY];
        Node target = graph[toX, toY];

        dist[source] = 0;
        prev[source] = null;

        bool targetFound = false;

        //initialize everything to have infinity distance 
        foreach (Node v in graph)
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
            Node u = null;
            foreach (Node possible in unvisited)
            {
                if (u == null || dist[possible] < dist[u])
                    u = possible;
            }

            if (u == target)
                break; //exit while loop if target is found

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                //float alt = dist[u] + u.DistTo(v);
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y, target, flying);

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

        List<Node> currentPath = new List<Node>();

        Node curr = target;

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
}
