#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilCommand : EditorCommand
{
    public PencilCommand(Coordinate coord) : base(coord)
    {
        this.type = UberManager.Instance.LevelEditor.CurrentPlacableType;

        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        if (type == LevelEditor.PlacableType.Content)
        {
            // undoredo
            redoMethod = RedoPlaceContent;
            undoMethod = UndoPlaceContent;

            // not my type
            prevTileType = (SecTileType)(-1);
            currTileType = (SecTileType)(-1);

            // my type
            prevContentType = (SecContentType)(-1);
            currContentType = UberManager.Instance.LevelEditor.SelectedContent.Value;
        }
        else if (type == LevelEditor.PlacableType.Tile)
        {
            redoMethod = RedoPlaceTile;
            undoMethod = UndoPlaceTile;

            // not my type
            prevTileType = tileNode == null ? (SecTileType)(-1) : tileNode.GetSecType();
            currTileType = UberManager.Instance.LevelEditor.SelectedTile.Value;

            // my type
            prevContentType = (SecContentType)(-1);
            currContentType = (SecContentType)(-1);
        }
    }

    public override bool Execute(Coordinate coord)
    {
        if (type == LevelEditor.PlacableType.Content) return PlaceContent();
        else if (type == LevelEditor.PlacableType.Tile) return PlaceTile();

        return false;
    }

    private bool PlaceTile()
    {
        bool notThisType = tileNode == null || tileNode.GetSecType() != currTileType;
        bool noContentWillBeDeleted = tileNode == null ||
                                      (tileNode.GetAmountOfContent() <= 0 && ContentManager.GetPrimaryFromSecTile(currTileType) == TileType.Dangerous) ||
                                      ContentManager.GetPrimaryFromSecTile(currTileType) != TileType.Dangerous;

        // if it's not already this tile type on this tile
        if (notThisType && noContentWillBeDeleted)
        {
            GameManager.Instance.TileManager.SetTileTypeDEVMODE(currTileType, coord);
            UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = currTileType;
            return true;
        }
        else if (!noContentWillBeDeleted)
        {
            Debug.LogError("Cannot place a dangerous tile underneath content");
            return false;
        }

        return false;
    }

    private bool UndoPlaceTile()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode != null)
        {
            UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = prevTileType;

            if (prevTileType == (SecTileType)(-1)) GameManager.Instance.TileManager.RemoveTileDEVMODE(coord);
            else GameManager.Instance.TileManager.SetTileTypeDEVMODE(prevTileType, coord);
            return true;
        }

        // should never ever happen if i can code
        return false;
    }

    private bool RedoPlaceTile()
    {
        GameManager.Instance.TileManager.SetTileTypeDEVMODE(currTileType, coord);
        UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = currTileType;
        return true;
    }

    private bool PlaceContent()
    {
        // if the node is null, there cannot be placed anything
        if (tileNode != null && tileNode.GetType() != TileType.Dangerous)
        {
            // cannot place content on top of each other!
            if (tileNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = ContentManager.GetPrimaryFromSecContent(currContentType);
                s.secType = currContentType;
                s.position = coord;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
                UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
                return true;
            }
            Debug.LogError("You cannot place content on top of another content object.");
            return false;
        }
        else
        {
            if (tileNode == null) Debug.LogError("You cannot place content on a non-existing tile.");
            else if (tileNode.GetType() == TileType.Dangerous) Debug.LogError("You cannot place content on a dangerous tile.");

            return false;
        }
    }

    private bool UndoPlaceContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode != null && tileNode.GetAmountOfContent() > 0)
        {
            SecContentType removedType = GameManager.Instance.TileManager.RemoveContentDEVMODE(tileNode);
            UberManager.Instance.LevelEditor.SpawnNodes.Remove(UberManager.Instance.LevelEditor.SpawnNodes.Find(s => s.secType == removedType && s.position == coord));
            return true;
        }

        // should never ever happen if i can code
        return false;
    }

    private bool RedoPlaceContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode != null && tileNode.GetAmountOfContent() == 0)
        {
            SpawnNode s = new SpawnNode();
            s.type = ContentManager.GetPrimaryFromSecContent(currContentType);
            s.secType = currContentType;
            s.position = coord;

            GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
            UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
            return true;
        }

        // should never ever happen if i can code
        return false;
    }
}
#endif