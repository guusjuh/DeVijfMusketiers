using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public List<Node> neighbours;
    public int x, y;

    public Node()
    {
        neighbours = new List<Node>();
    }

    //return the actual distance to make pathfinding look better,
    //but cost will eventually also be 1 for diagonals 
    public float DistTo(Node n)
    {
        return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
    }
}