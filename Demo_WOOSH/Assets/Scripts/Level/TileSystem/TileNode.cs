using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode {
    private Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    private Vector3 worldPosition;
    public Vector3 WorldPosition { get { return worldPosition; } }

    private List<TileNode> neighbours;
    public List<TileNode> NeightBours { get { return neighbours; } }

    private GameObject hexagon;
    public GameObject Hexagon { get { return hexagon; } }

    private SpriteRenderer highlight;
    public SpriteRenderer Highlight { get { return highlight; } }

    private TileContent content;
    public TileContent Content { get { return content; } }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="gridPosition">The grid position.</param>
    /// <param name="worldPosition">The world position.</param>
    public TileNode(Coordinate gridPosition, Vector3 worldPosition, SecTileType type)
    {
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;

        neighbours = new List<TileNode>();

        content = new TileContent(this, type);

        // Create a hexagon of the given type and store a reference.
        CreateHexagon(type);
    }

    public void Clear()
    {
        neighbours.Clear();
        neighbours = null;

        GameObject.Destroy(hexagon);
    }

    /// <summary>
    /// Create a hexagon gameobject.
    /// </summary>
    /// <param name="hexagonType">The type of the hexagon.</param>
    /// <returns></returns>
    public void CreateHexagon(SecTileType type)
    {
        // first call hexagon will be null, still create 
        if (content.SecTileType == type && hexagon != null) return;

        if (hexagon != null) GameObject.Destroy(hexagon);

        content.SecTileType = type;

        Object prefab = ContentManager.Instance.TilePrefabs[new KeyValuePair<TileType, SecTileType>
                                                            (ContentManager.GetPrimaryFromSecTile(type), type)];

        hexagon = GameObject.Instantiate(prefab) as GameObject;
        hexagon.transform.parent = GameManager.Instance.TileManager.GridParent.transform;
        hexagon.name = prefab.name;

        // Set the position.
        hexagon.transform.position = worldPosition;

        highlight = hexagon.transform.Find("Highlight").GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Search for neighbour nodes. 
    /// </summary>
    public void SearchNeighbours()
    {
        // Check for neighbours in every possible direction. 
        for (int i = 0; i < GameManager.Instance.TileManager.Directions(gridPosition).Length; i++)
        {
            // The position of the searched neighbour
            Coordinate tempPos = gridPosition + GameManager.Instance.TileManager.Directions(gridPosition)[i];

            // If this position is greater than the minimum of the grid.And if this position is smaller than the maximum of the grid.
            if (tempPos.x >= 0 && tempPos.y >= 0 && tempPos.x < GameManager.Instance.TileManager.Rows &&
                tempPos.y < GameManager.Instance.TileManager.Columns)
            {
                // Check for the position containing a node and add it to the list of neighbours.
                if (GameManager.Instance.TileManager.GetNodeReference(tempPos) != null)
                    neighbours.Add(GameManager.Instance.TileManager.GetNodeReference(tempPos));
            }
        }
    }

    public void HighlightTile(bool on, Color color = default(Color))
    {
        highlight.gameObject.SetActive(on);
        if (on) highlight.color = color;
    }

    public float DistanceFromPosition(Vector3 position)
    {
        return Vector3.Distance(position, worldPosition);
    }

    public SecTileType GetSecType()
    {
        return content.SecTileType;
    }

    public TileType GetType()
    {
        return ContentManager.GetPrimaryFromSecTile(content.SecTileType);
    }

    public int GetAmountOfContent()
    {
        return content.ContentTypes.Count;
    }

    public List<WorldObject> GetContent()
    {
        return content.ContentTypes;
    }

    public bool ContainsHuman()
    {
        return content.ContentTypes.Find(c => c.IsHuman()) != null;
    }

    public bool ContainsMonster()
    {
        return content.ContentTypes.Find(c => c.IsMonster()) != null;
    }

    public bool ContainsWalkingMonster()
    {
        return content.ContentTypes.Find(c => c.IsMonster() && c.IsWalking()) != null;
    }

    public bool ContainsFlyingMonster()
    {
        return content.ContentTypes.Find(c => c.IsMonster() && c.IsFlying()) != null;
    }

    public bool ContainsShrine()
    {
        return content.ContentTypes.Find(c => c.IsShrine()) != null;
    }

    public bool ContainsBarrel()
    {
        return content.ContentTypes.Find(c => c.IsBarrel()) != null;
    }

    public bool ContainsBrokenBarrel()
    {
        return content.ContentTypes.Find(c => c.IsBarrel() && c.GetComponent<Rock>().Destroyed) != null;
    }

    public bool CompletelyEmpty()
    {
        return content.ContentTypes.Count == 0 && ContentManager.GetPrimaryFromSecTile(content.SecTileType) != TileType.Dangerous;
    }

    public bool WalkAble()
    {
        bool containsBrokenBarrel = content.ContentTypes.Count == 1 &&
                                    content.ContentTypes[0].IsBarrel() &&
                                    content.ContentTypes[0].GetComponent<Rock>().Destroyed &&
                                    ContentManager.GetPrimaryFromSecTile(content.SecTileType) != TileType.Dangerous;

        return CompletelyEmpty() || containsBrokenBarrel;
    }

    public int EnterCost()
    {
        return content.EnterCost();
    }

    public void AddContent(WorldObject worldObject)
    {
        content.AddContent(worldObject);
    }

    public void RemoveContent(WorldObject worldObject)
    {
        content.RemoveContent(worldObject);
    }

    public WorldObject RemoveContent()
    {
        return content.RemoveContent();
    }
}
