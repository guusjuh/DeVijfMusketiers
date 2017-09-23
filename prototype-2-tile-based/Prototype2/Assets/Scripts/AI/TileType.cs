using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType
{
    public TileMap.Types type;
    public GameObject prefab;
    public float movementCost = 1;
    public bool canEnter = true;
    public bool canFlyOver = true;

}
