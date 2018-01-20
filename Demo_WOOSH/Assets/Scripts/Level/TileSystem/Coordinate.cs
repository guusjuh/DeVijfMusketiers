using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2 ToVector2() 
    {
        return new Vector2(this.x, this.y);
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

    public float EuclideanDistance(Coordinate other)
    {
        Vector2 thisPos = GameManager.Instance.TileManager.GetWorldPosition(this);
        Vector2 otherPos = GameManager.Instance.TileManager.GetWorldPosition(other);

        return Vector2.Distance(otherPos, thisPos);
    }
}
