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
        Vase,
        BrokenVase
    }

    [SerializeField]
    public TileType[] tileTypes;
    private Node[,] graph;
    private Types[,] tiles;

    private int columns, rows;

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

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY, Node endTarget)
    {
        TileType tt = tileTypes[(int)tiles[targetX, targetY]];

        if (new Vector2(endTarget.x - targetX, endTarget.y - targetY).magnitude > 0.1f)
        {
            if (!UnitCanEnterTile(targetX, targetY))
                return Mathf.Infinity;
        }

        float cost = tt.movementCost;

        //adding very tiny extra cost to diagonals, looks better
        if (sourceX != targetX && sourceY != targetY)
            cost += 0.001f;

        return cost;
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    public bool UnitCanEnterTile(int x, int y)
    {
        //possible to check for walk/hover/fly skills or whatever against terrain types
        return tileTypes[(int)tiles[x, y]].canEnter;
    }

    public void RemoveObject(int x, int y)
    {
        tiles[x, y] = Types.Empty;
    }

    public void SwitchVaseStatus(int x, int y, bool broken)
    {
        // cannot walk on vases, not even when broken when commented out
        //tiles[x, y] = broken ? Types.BrokenVase : Types.Vase;
    }

    public List<Node> GeneratePathTo(int fromX, int fromY, int toX, int toY)
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
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y, target);

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
