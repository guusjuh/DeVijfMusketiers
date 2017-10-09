using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileContent
{
    private TileManager.TileType tileType = TileManager.TileType.Normal;
    public TileManager.TileType TileType { get { return tileType; } }

    private List<TileManager.ContentType> contentTypes = new List<TileManager.ContentType>();
    public List<TileManager.ContentType> ContentTypes { get { return contentTypes; } }

    private TileNode refNode;

    public TileContent(TileNode refNode)
    {
        this.refNode = refNode;
    }

    public bool CompletelyEmpty()
    {
        return contentTypes.Count == 0 && tileType != TileManager.TileType.Goo;
    }

    public void SetTileType(TileManager.TileType type)
    {
        // maybe this check isn't needed in the future,
        // but for now, you cannot change back from goo

        if (tileType == TileManager.TileType.Goo)
            return;
        else
        {
            tileType = type;
            if (tileType == TileManager.TileType.Goo) refNode.MakeGoo();

            // kill all on this tile!
            if (contentTypes.Contains(TileManager.ContentType.Barrel) ||
                contentTypes.Contains(TileManager.ContentType.BrokenBarrel))
            {
                GameManager.Instance.LevelManager.Barrels.Find(go => go.GridPosition == refNode.GridPosition).RemoveByGoo();
            }
            else if (contentTypes.Contains(TileManager.ContentType.Human) ||
                contentTypes.Contains(TileManager.ContentType.InivisbleHuman))
            {
                GameManager.Instance.LevelManager.Humans.Find(go => go.GridPosition == refNode.GridPosition).Hit();
            }
            else if (contentTypes.Contains(TileManager.ContentType.Shrine))
            {
                GameManager.Instance.LevelManager.Shrines.Find(go => go.GridPosition == refNode.GridPosition).Hit();
            }
        }
        
    }

    public void AddContent(TileManager.ContentType type)
    {
        contentTypes.Add(type);
    }

    public void RemoveContent(TileManager.ContentType type)
    {
        contentTypes.Remove(type);
    }

    public void ReplaceContent(TileManager.ContentType oldType, TileManager.ContentType newType)
    {
        RemoveContent(oldType);
        AddContent(newType);
    }

    public int EnterCost()
    {
        int cost = 1;

        if (contentTypes.Contains(TileManager.ContentType.Barrel) ||
                contentTypes.Contains(TileManager.ContentType.Human) ||
                contentTypes.Contains(TileManager.ContentType.Shrine))
        {
            cost++;
        }
        
        return cost;
    }

    public bool CanEnter(bool flying = false)
    {
        //TODO: loop through content and decide Y/N
        return true;
    }

}
