using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: a solid method to find the difference between two coordinates (not in worldspace, but coordinatespace)
[Serializable]
public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinate(Vector2 vec)
    {
        this.x = (int) vec.x;
        this.y = (int) vec.y;
    }

    public static Coordinate zero { get { return new Coordinate(0,0);} }

    public static Coordinate operator +(Coordinate c1, Coordinate c2)
    {
        return new Coordinate(c1.x + c2.x, c1.y + c2.y);
    }

    public static Coordinate operator -(Coordinate c1, Coordinate c2)
    {
        return new Coordinate(c1.x - c2.x, c1.y - c2.y);
    }

    public static bool operator ==(Coordinate c1, Coordinate c2)
    {
        return c1.x == c2.x && c1.y == c2.y ? true : false;
    }

    public static bool operator !=(Coordinate c1, Coordinate c2)
    {
        return c1.x != c2.x || c1.y != c2.y ? true : false;
    }

    public int ManhattanDistanceOld(Coordinate other)
    {
        return Mathf.Abs((this.x - other.x)) + Mathf.Abs((this.y - other.y));
    }

    public float ManhattanDistance(Coordinate other)
    {
        Vector2 thisPos = GameManager.Instance.TileManager.GetWorldPosition(this);
        Vector2 otherPos = GameManager.Instance.TileManager.GetWorldPosition(other);

        return Mathf.Abs((thisPos.x - otherPos.x)) + Mathf.Abs((thisPos.y - otherPos.y));
    }
}
