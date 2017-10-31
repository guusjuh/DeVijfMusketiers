using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCommand : EditorCommand
{
    private List<Coordinate> neighbours = new List<Coordinate>();

    public FillCommand(Coordinate coord) : base(coord)
    {
        this.type = UberManager.Instance.LevelEditor.CurrentPlacableType;

        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        if (type == LevelEditor.PlacableType.Content)
        {
            // undoredo
            redoMethod = RedoFillContent;
            undoMethod = UndoFillContent;

            // not my type
            prevTileType = (SecTileType)(-1);
            currTileType = (SecTileType)(-1);

            // my type
            prevContentType = (SecContentType)(-1);
            currContentType = UberManager.Instance.LevelEditor.SelectedContent.Value;
        }
        else if (type == LevelEditor.PlacableType.Tile)
        {
            redoMethod = RedoFillTile;
            undoMethod = UndoFillTile;

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
        if (type == LevelEditor.PlacableType.Content) return FillContent(coord);
        else if (type == LevelEditor.PlacableType.Tile) return FillTile(coord);

        return false;
    }

    private bool FillTile(Coordinate newCoord)
    {
        TileNode nextTileNode = GameManager.Instance.TileManager.GetNodeReference(newCoord);

        bool alreadyThisTileType = nextTileNode != null && nextTileNode.GetSecType() == currTileType;
        bool sameTypeAsInitial = true;

        if (prevTileType == SecTileType.Unknown && nextTileNode == null)
            sameTypeAsInitial = true;
        else if (nextTileNode != null && nextTileNode.GetSecType() == prevTileType)
            sameTypeAsInitial = true;
        else
            sameTypeAsInitial = false;

        bool noContentWillBeDeleted = nextTileNode == null ||
                                      (nextTileNode.GetAmountOfContent() <= 0 && ContentManager.GetPrimaryFromSecTile(currTileType) == TileType.Dangerous) ||
                                      ContentManager.GetPrimaryFromSecTile(currTileType) != TileType.Dangerous;

        if (!UberManager.Instance.LevelEditor.ValidPosition(newCoord) || alreadyThisTileType || !sameTypeAsInitial || !noContentWillBeDeleted) return false;

        if (coord != newCoord) neighbours.Add(newCoord);
        PlaceTile(newCoord);

        Coordinate[] directions = GameManager.Instance.TileManager.Directions(newCoord);

        for (int i = 0; i < directions.Length; i++)
        {
            FillTile(newCoord + directions[i]);
        }

        return true;
    }

    private void PlaceTile(Coordinate coord)
    {
        TileNode nextTileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        bool notThisType = nextTileNode == null || nextTileNode.GetSecType() != currTileType;
        bool noContentWillBeDeleted = nextTileNode == null ||
                                      (nextTileNode.GetAmountOfContent() <= 0 && ContentManager.GetPrimaryFromSecTile(currTileType) == TileType.Dangerous) ||
                                      ContentManager.GetPrimaryFromSecTile(currTileType) != TileType.Dangerous;

        // if it's not already this tile type on this tile
        if (notThisType && noContentWillBeDeleted)
        {
            GameManager.Instance.TileManager.SetTileTypeDEVMODE(currTileType, coord);
            UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = currTileType;
        }
        else if (!noContentWillBeDeleted)
        {
            Debug.LogError("Cannot place a dangerous tile underneath content");
        }
    }

    private bool UndoFillTile()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        TileNode currentTileNode = tileNode;

        for (int i = -1; i < neighbours.Count; i++)
        {
            // update tilenode
            if (i >= 0) currentTileNode = GameManager.Instance.TileManager.GetNodeReference(neighbours[i]);

            // check for not being null
            if (currentTileNode != null)
            {
                UberManager.Instance.LevelEditor.Grid[currentTileNode.GridPosition.x].row[currentTileNode.GridPosition.y] = prevTileType;

                if (prevTileType == (SecTileType)(-1)) GameManager.Instance.TileManager.RemoveTileDEVMODE(currentTileNode.GridPosition);
                else GameManager.Instance.TileManager.SetTileTypeDEVMODE(prevTileType, currentTileNode.GridPosition);
            }
        }

        return true;

        // should never ever happen if i can code
        //return false;
    }

    private bool RedoFillTile()
    {
        Coordinate currentCoord = coord;

        for (int i = -1; i < neighbours.Count; i++)
        {
            if (i >= 0) currentCoord = neighbours[i];

            GameManager.Instance.TileManager.SetTileTypeDEVMODE(currTileType, currentCoord);
            UberManager.Instance.LevelEditor.Grid[currentCoord.x].row[currentCoord.y] = currTileType;
        }

        return true;
    }

    private bool FillContent(Coordinate newCoord)
    {
        TileNode nextTileNode = GameManager.Instance.TileManager.GetNodeReference(newCoord);

        bool alreadyContent = nextTileNode != null && nextTileNode.GetAmountOfContent() > 0;
        bool cannotPlaceContent = (nextTileNode != null && nextTileNode.GetType() == TileType.Dangerous) || nextTileNode == null;

        bool occupied = alreadyContent || cannotPlaceContent;

        if (!UberManager.Instance.LevelEditor.ValidPosition(newCoord) || occupied) return false;

        if(coord != newCoord) neighbours.Add(newCoord);

        PlaceContent(newCoord);

        Coordinate[] directions = GameManager.Instance.TileManager.Directions(newCoord);

        for (int i = 0; i < directions.Length; i++)
        {
            FillContent(newCoord + directions[i]);
        }

        return true;
    }

    private void PlaceContent(Coordinate coord)
    {
        TileNode nextTileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node is null, there cannot be placed anything
        if (nextTileNode != null && nextTileNode.GetType() != TileType.Dangerous)
        {
            // cannot place content on top of each other!
            if (nextTileNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = ContentManager.GetPrimaryFromSecContent(currContentType);
                s.secType = currContentType;
                s.position = coord;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
                UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
                return;
            }
            Debug.LogError("You cannot place content on top of another content object.");
        }
        else
        {
            if (nextTileNode == null) Debug.LogError("You cannot place content on a non-existing tile.");
            else if (nextTileNode.GetType() == TileType.Dangerous) Debug.LogError("You cannot place content on a dangerous tile.");
        }
    }

    private bool UndoFillContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        TileNode currentTileNode = tileNode;

        for (int i = -1; i < neighbours.Count; i++)
        {
            // update tilenode
            if (i >= 0) currentTileNode = GameManager.Instance.TileManager.GetNodeReference(neighbours[i]);

            // check for not being null
            if (currentTileNode != null && currentTileNode.GetAmountOfContent() > 0)
            {
                SecContentType removedType = GameManager.Instance.TileManager.RemoveContentDEVMODE(currentTileNode);
                UberManager.Instance.LevelEditor.SpawnNodes.Remove(UberManager.Instance.LevelEditor.SpawnNodes.Find(s => s.secType == removedType && s.position == coord));
            }
        }

        return true;
    }

    private bool RedoFillContent()
    {
        // remember that the tilenode can be deleted, so we might lose our reference
        tileNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        TileNode currentTileNode = tileNode;

        for (int i = -1; i < neighbours.Count; i++)
        {
            // update tilenode
            if (i >= 0) currentTileNode = GameManager.Instance.TileManager.GetNodeReference(neighbours[i]);

            // check for not being null
            if (currentTileNode != null && currentTileNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = ContentManager.GetPrimaryFromSecContent(currContentType);
                s.secType = currContentType;
                s.position = currentTileNode.GridPosition;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
                UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
            }
        }

        return true;
    }
}
