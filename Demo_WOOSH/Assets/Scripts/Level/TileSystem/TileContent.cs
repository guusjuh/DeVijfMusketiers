using System.Collections.Generic;

public class TileContent
{
    private SecTileType tileType = SecTileType.Dirt;
    public SecTileType SecTileType { get { return tileType; }
        set
        {
            if(value != tileType && ContentManager.GetPrimaryFromSecTile(value) == TileType.Dangerous)
                MakeTileDangerous(value);

            tileType = value;
        }
    }

    private List<WorldObject> contentTypes = new List<WorldObject>();
    public List<WorldObject> ContentTypes { get { return contentTypes; } }

    private TileNode refNode;

    public TileContent(TileNode refNode, SecTileType tileType)
    {
        this.refNode = refNode;
        this.tileType = tileType;
    }

    public int EnterCost()
    {
        int cost = 1;

        if (contentTypes.Find(c => c.IsBarrel()) ||
            contentTypes.Find(c => c.IsShrine()) ||
            contentTypes.Find(c => c.IsHuman()))
        {
            cost++;
        }

        return cost;
    }

    //TODO: enable spawning different kinds of dangerous tiles
    private void MakeTileDangerous(SecTileType newType)
    {
        // dont change if this tile is already dangerous
        if (ContentManager.GetPrimaryFromSecTile(tileType) == TileType.Dangerous) return;

        // dont change to a normal tile
        if (ContentManager.GetPrimaryFromSecTile(newType) == TileType.Normal) return;

        // change to new type
        tileType = newType;

        // kill all on this tile!
        KillTileContent();
    }

    private void KillTileContent()
    {
        if (contentTypes.Find(c => c.IsBarrel())) GameManager.Instance.LevelManager.Barrels.Find(go => go.GridPosition == refNode.GridPosition).DeadByGap();
        if (contentTypes.Find(c => c.IsHuman())) GameManager.Instance.LevelManager.Humans.Find(go => go.GridPosition == refNode.GridPosition).DeadByGap();
        if (contentTypes.Find(c => c.IsShrine())) GameManager.Instance.LevelManager.Shrines.Find(go => go.GridPosition == refNode.GridPosition).DeadByGap();
    }

    public void AddContent(WorldObject worldObject)
    {
        contentTypes.Add(worldObject);
    }

    public void RemoveContent(WorldObject worldObject)
    {
        contentTypes.Remove(worldObject);
    }

    public WorldObject RemoveContent()
    {
        WorldObject returnObj = contentTypes.Last();
        contentTypes.Remove(returnObj);
        return returnObj;
    }
}
