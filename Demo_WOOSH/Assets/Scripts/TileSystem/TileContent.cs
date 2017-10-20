using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class TileContent
{
    private TileManager.TileType tileType = TileManager.TileType.Normal;
    public TileManager.TileType TileType { get { return tileType; } }

    private List<WorldObject> contentTypes = new List<WorldObject>();
    public List<WorldObject> ContentTypes { get { return contentTypes; } }

    private TileNode refNode;

    public TileContent(TileNode refNode)
    {
        this.refNode = refNode;
    }

    public bool ContainsHuman()
    {
        return contentTypes.Find(c => c.IsHuman()) != null;
    }

    public bool ContainsMonster()
    {
        return contentTypes.Find(c => c.IsMonster()) != null;
    }

    public bool ContainsWalkingMonster()
    {
        return contentTypes.Find(c => c.IsMonster() && c.IsWalking()) != null;
    }

    public bool ContainsFlyingMonster()
    {
        return contentTypes.Find(c => c.IsMonster() && c.IsFlying()) != null;
    }

    public bool ContainsShrine()
    {
        return contentTypes.Find(c => c.IsShrine()) != null;
    }

    public bool ContainsBarrel()
    {
        return contentTypes.Find(c => c.IsBarrel()) != null;
    }

    public bool ContainsBrokenBarrel()
    {
        return contentTypes.Find(c => c.IsBarrel() && c.GetComponent<Barrel>().Destroyed) != null;
    }

    public bool CompletelyEmpty()
    {
        return contentTypes.Count == 0 && tileType != TileManager.TileType.Dangerous;
    }

    public bool WalkAble()
    {
        bool containsBrokenBarrel = contentTypes.Count == 1 &&
                                    contentTypes[0].IsBarrel() &&
                                    contentTypes[0].GetComponent<Barrel>().Destroyed &&
                                    tileType != TileManager.TileType.Dangerous;

        return CompletelyEmpty() || containsBrokenBarrel;
    }

    public void SetTileType(TileManager.TileType type)
    {
        // maybe this check isn't needed in the future,
        // but for now, you cannot change back from goo

        if (tileType == TileManager.TileType.Dangerous)
            return;
        else
        {
            tileType = type;
            if (tileType == TileManager.TileType.Dangerous) refNode.MakeGap();

            // kill all on this tile!
            if (contentTypes.Find(c => c.IsBarrel()))
            {
                GameManager.Instance.LevelManager.Barrels.Find(go => go.GridPosition == refNode.GridPosition).RemoveByGap();
            }
            else if (contentTypes.Find(c => c.IsHuman()))
            {
                GameManager.Instance.LevelManager.Humans.Find(go => go.GridPosition == refNode.GridPosition).Hit();
            }
            else if(contentTypes.Find(c => c.IsShrine()))
            {
                GameManager.Instance.LevelManager.Shrines.Find(go => go.GridPosition == refNode.GridPosition).Hit();
            }
        }
        
    }

    public void AddContent(WorldObject worldObject)
    {
        contentTypes.Add(worldObject);
    }

    public void RemoveContent(WorldObject worldObject)
    {
        contentTypes.Remove(worldObject);
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
}
