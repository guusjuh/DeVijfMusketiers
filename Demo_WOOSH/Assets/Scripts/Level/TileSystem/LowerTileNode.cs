using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerTileNode : TileNode
{
    private const float OBJECT_CHANCE = 0.5f;

    private bool higherOccupied = true;
    public bool HigherOccupied { get { return higherOccupied; } }

    private List<LowerTileNode> neighbours;
    public List<LowerTileNode> Neighbours { get { return neighbours; } }

    public LowerTileNode(Coordinate gridPosition, Vector3 worldPosition, bool occupied) : base(gridPosition, worldPosition)
    {
        neighbours = new List<LowerTileNode>();

        higherOccupied = occupied;

        CreateHexagon();
    }

    public void HigherDestroyed()
    {
        if (!higherOccupied) return;

        higherOccupied = false;
        hexagon.SetActive(true);
    }

    private void CreateHexagon()
    {
        if (hexagon != null) GameObject.Destroy(hexagon);

        Object prefab = Resources.Load<Object>("Prefabs/Tiles/LowerTile");

        hexagon = GameObject.Instantiate(prefab) as GameObject;
        hexagon.transform.parent = GameManager.Instance.TileManager.GridParent.transform;
        hexagon.name = prefab.name;

        // Set the position.
        hexagon.transform.position = worldPosition;

        AssignTileSprite();
        if (!GameManager.Instance.TileManager.LowerTileManager.NoContent(gridPosition)) AssignObjectSprite();

        if (higherOccupied) hexagon.SetActive(false);
    }

    private void AssignTileSprite()
    {
        if (hexagon == null) return;

        hexagon.GetComponent<SpriteRenderer>().sprite
            = ContentManager.GetRandomEnvironmentTile(ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).environmentType);
    }

    private void AssignObjectSprite()
    {
        if (hexagon == null) return;

        float r = Random.Range(0, 100) / 100.0f;

        if (r <= OBJECT_CHANCE)
        {
            Transform child = hexagon.transform.Find("Environment");
            child.gameObject.SetActive(true);
            child.GetComponent<SpriteRenderer>().sprite
                = ContentManager.GetRandomenvironmentObject(ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).environmentType);
        }
    }
}
