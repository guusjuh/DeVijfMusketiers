#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCommand : Command<Coordinate>
{
    protected LevelTileNode tileNode;
    protected Coordinate coord;
    protected LevelEditor.PlacableType type;

    public delegate bool UndoMethod();
    public delegate bool RedoMethod();

    public UndoMethod undoMethod;
    public RedoMethod redoMethod;

    // can be -1 if type = tile
    protected SecContentType prevContentType;
    protected SecContentType currContentType;

    // can be -1 if type = content
    protected SecTileType prevTileType;
    protected SecTileType currTileType;

    public EditorCommand(Coordinate coord)
    {
        this.coord = coord;
    }
}
#endif