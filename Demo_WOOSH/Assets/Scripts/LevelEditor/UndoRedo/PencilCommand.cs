using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilCommand : EditorCommand
{
    public PencilCommand(Coordinate coord) : base(coord)
    {
        this.type = UberManager.Instance.LevelEditor.CurrentPlacableType;

        if(type == LevelEditor.PlacableType.Content) undoMethod = PlaceContent;
        if(type == LevelEditor.PlacableType.Tile) undoMethod = PlaceTile;

        //TODO:NOW redo delegate
    }

    public override bool Execute(Coordinate coord)
    {
        return false;
    }

    public bool PlaceTile()
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        bool notThisType = existingNode == null ||
                           existingNode.GetSecType() != UberManager.Instance.LevelEditor.SelectedTile.Value;
        bool noContentWillBeDeleted = existingNode == null ||
                                      (existingNode.GetAmountOfContent() <= 0 &&
                                      UberManager.Instance.LevelEditor.SelectedTile.Key == TileType.Dangerous) ||
                                      UberManager.Instance.LevelEditor.SelectedTile.Key != TileType.Dangerous;

        // if it's not already this tile type on this tile
        if (notThisType && noContentWillBeDeleted)
        {
            GameManager.Instance.TileManager.SetTileTypeDEVMODE(UberManager.Instance.LevelEditor.SelectedTile.Value, coord);
            UberManager.Instance.LevelEditor.Grid[coord.x].row[coord.y] = UberManager.Instance.LevelEditor.SelectedTile.Value;
            return true;
        }
        else if (!noContentWillBeDeleted)
        {
            Debug.LogError("Cannot place a dangerous tile underneath content");
            return false;
        }

        return false;
    }

    public bool PlaceContent()
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node is null, there cannot be placed anything
        if (existingNode != null && existingNode.GetType() != TileType.Dangerous)
        {
            // cannot place content on top of each other!
            if (existingNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = UberManager.Instance.LevelEditor.SelectedContent.Key;
                s.secType = UberManager.Instance.LevelEditor.SelectedContent.Value;
                s.position = coord;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
                UberManager.Instance.LevelEditor.SpawnNodes.Add(s);
                return true;
            }
            return false;
        }
        else
        {
            if (existingNode == null) Debug.LogError("You cannot place content on a non-existing tile.");
            else if (existingNode.GetType() == TileType.Dangerous) Debug.LogError("You cannot place content on a dangerous tile.");

            return false;
        }
    }
}
