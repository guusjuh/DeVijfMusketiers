using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContent
{
    private TileManager.TileType tileType = TileManager.TileType.Normal;
    public TileManager.TileType TileType { get { return tileType; } }

    private List<TileManager.ContentType> contentTypes = new List<TileManager.ContentType>();
    public List<TileManager.ContentType> ContentTypes { get { return contentTypes; } }

    public bool CompletelyEmpty()
    {
        return contentTypes.Count == 0;
    }

    public void SetTileType(TileManager.TileType type)
    {
        // maybe this check isn't needed in the future,
        // but for now, you cannot change back from goo

        if(tileType == TileManager.TileType.Goo)
            return;

        tileType = type;
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
        //TODO: add all costs
        return 1;
    }

    public bool CanEnter(bool flying = false)
    {
        //TODO: loop through content and decide Y/N
        return true;
    }

}
