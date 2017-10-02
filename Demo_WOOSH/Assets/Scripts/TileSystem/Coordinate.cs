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
}
