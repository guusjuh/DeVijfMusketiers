using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedTypeData {
    /// <summary>
    /// The previous selected tile type, used to set when switched between placement type to tile
    /// </summary>
    public TileType prevTile;
    /// <summary>
    /// The previous selected content type, used to set when switched between placement type to content
    /// </summary>
    public ContentType prevContent;

    /// <summary>
    /// The previous selected secondairy tile type, used to set when switched between primary type
    /// </summary>
    public Dictionary<TileType, SecTileType> prevSecTiles;
    /// <summary>
    /// The previous selected secondairy content type, used to set when switched between primary type
    /// </summary>
    public Dictionary<ContentType, SecContentType> prevSecContents;

    /// <summary>
    /// The currently selected tile
    /// </summary>
    public KeyValuePair<TileType, SecTileType> selectedTile;
    /// <summary>
    /// The currently selected content
    /// </summary>
    public KeyValuePair<ContentType, SecContentType> selectedContent;

    public void Initialize()
    {
        // set prev primary type
        prevTile = (TileType)0;
        prevContent = (ContentType)0;

        // set each type initially to the first of its kind
        prevSecTiles = new Dictionary<TileType, SecTileType>();
        for (int i = 0; i < Enum.GetValues(typeof(TileType)).Length - 1; i++)
        {
            prevSecTiles.Add((TileType)i, ContentManager.ValidTileTypes[(TileType)i][0]);
        }

        prevSecContents = new Dictionary<ContentType, SecContentType>();
        for (int i = 0; i < Enum.GetValues(typeof(ContentType)).Length - 1; i++)
        {
            prevSecContents.Add((ContentType)i, ContentManager.ValidContentTypes[(ContentType)i][0]);
        }

        // select the first tile and the first content types
        selectedTile = prevSecTiles.GetEntry(prevTile);
        selectedContent = prevSecContents.GetEntry(prevContent);
    }
}
