#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteCommand : EditorCommand
{
    public DeleteCommand(Coordinate coord) : base(coord)
    {
        this.type = UberManager.Instance.LevelEditor.CurrentPlacableType;

        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        if (type == LevelEditor.PlacableType.Content)
        {
            // undoredo
            redoMethod = DeleteContent;
            undoMethod = UndoDeleteContent;

            // not my type
            prevTileType = (SecTileType)(-1);
            currTileType = (SecTileType)(-1);

            // my type
            if (tileNode == null || tileNode.GetAmountOfContent() <= 0) prevContentType = (SecContentType) (-1);
            else prevContentType = tileNode.GetContent().Last().Type;
            currContentType = (SecContentType)(-1);
        }
        else if (type == LevelEditor.PlacableType.Tile)
        {
            redoMethod = RedoDeleteTile;
            undoMethod = UndoDeleteTile;

            // not my type
            prevTileType = tileNode == null ? (SecTileType)(-1) : tileNode.GetSecType();
            currTileType = (SecTileType)(-1);

            // my type
            prevContentType = (SecContentType)(-1);
            currContentType = (SecContentType)(-1);
        }
    }

    public override bool Execute(Coordinate coord)
    {
        if (type == LevelEditor.PlacableType.Content) return DeleteContent();
        else if (type == LevelEditor.PlacableType.Tile) return DeleteTile();

        return false;
    }

    private bool DeleteTile()
    {
        // if the node actually exists, delete it
        if (tileNode != null)
        {
            if (tileNode.GetAmountOfContent() <= 0)
            {
                GameManager.Instance.TileManager.RemoveTileDEVMODE(coord);
                UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = SecTileType.Unknown;
                return true;
            }
            else
            {
                Debug.LogError("Cannot remove a tile with content on it.");
                return false;
            }
        }

        return false;
    }

    private bool UndoDeleteTile()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode == null)
        {
            UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = prevTileType;
            GameManager.Instance.TileManager.SetTileTypeDEVMODE(prevTileType, coord);
            return true;
        }

        // should never ever happen if i can code
        return false;
    }

    private bool RedoDeleteTile()
    {
        GameManager.Instance.TileManager.RemoveTileDEVMODE(coord);
        UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = SecTileType.Unknown;
        return true;
    }

    private bool DeleteContent()
    {
        // if the node actually exists, delete it
        if (tileNode != null && tileNode.GetAmountOfContent() > 0)
        {
            SecContentType removedType = GameManager.Instance.TileManager.RemoveContentDEVMODE(tileNode);
            UberManager.Instance.LevelEditor.SpawnNodes.Remove(UberManager.Instance.LevelEditor.SpawnNodes.Find(s => s.secType == removedType && s.position == coord));

            return true;
        }

        return false;
    }

    private bool UndoDeleteContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode != null && tileNode.GetAmountOfContent() <= 0)
        {
            SpawnNode s = new SpawnNode();
            s.type = ContentManager.GetPrimaryFromSecContent(prevContentType);
            s.secType = prevContentType;
            s.position = coord;

            GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
            UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
            return true;
        }

        // should never ever happen if i can code
        return false;
    }

    private bool RedoDeleteContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // check for not being null
        if (tileNode != null && tileNode.GetAmountOfContent() == 0)
        {
            SecContentType removedType = GameManager.Instance.TileManager.RemoveContentDEVMODE(tileNode);
            UberManager.Instance.LevelEditor.SpawnNodes.Remove(UberManager.Instance.LevelEditor.SpawnNodes.Find(s => s.secType == removedType && s.position == coord));

            return true;
        }

        // should never ever happen if i can code
        return false;
    }
}
#endif