using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEditor : MonoBehaviour
{
    public enum PlacableType
    {
        Tile,
        Content
    }

    public enum ToolType
    {
        Brush,
        Fill
    }

    private GridOverlay gridOverlay;

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }

    private ToolType toolType = ToolType.Brush;

    private PlacableType placableType = PlacableType.Tile;
    private Transform previewObject; // the preview stuck on the mouse

    /// <summary>
    /// The previous selected tile type, used to set when switched between placement type to tile
    /// </summary>
    private TileType prevTileType;
    /// <summary>
    /// The previous selected content type, used to set when switched between placement type to content
    /// </summary>
    private ContentType prevContentType;

    /// <summary>
    /// The previous selected secondairy tile type, used to set when switched between primary type
    /// </summary>
    private Dictionary<TileType, SecTileType> prevSecTileTypes;
    /// <summary>
    /// The previous selected secondairy content type, used to set when switched between primary type
    /// </summary>
    private Dictionary<ContentType, SecContentType> prevSecContentTypes;

    private KeyValuePair<TileType, SecTileType> selectedTile; // the pair to access the go from contentmanager
    private KeyValuePair<ContentType, SecContentType> selectedContent; // the pair to access the go from contentmanager

    private Vector3 worldMousePosition;
    private Coordinate coordinateMousePosition;
    Texture2D fillCursor;
    Texture2D pencilCursor;

    public void Initialize()
    {
        // set initial rows and columns
        rows = 7;
        columns = 9;

        // setup grid overlay
        gridOverlay = UberManager.Instance.gameObject.AddComponent<GridOverlay>();
        gridOverlay.Initialize();

        // setup mouse
        worldMousePosition = Vector3.zero;
        coordinateMousePosition = Coordinate.zero;
        fillCursor = Resources.Load<Texture2D>("Sprites/LevelEditor/fill");
        pencilCursor = Resources.Load<Texture2D>("Sprites/LevelEditor/pencil");

        // set intial tooltype
        ChangeToolType(ToolType.Fill);

        // set intial selected 
        placableType = PlacableType.Tile;

        // set each type initially to the first of its kind
        prevSecContentTypes = new Dictionary<ContentType, SecContentType>();
        for (int i = 0; i < Enum.GetValues(typeof(ContentType)).Length - 1; i++)
        {
            prevSecContentTypes.Add((ContentType) i, ContentManager.ValidContentTypes[(ContentType) i][0]);
        }

        prevSecTileTypes = new Dictionary<TileType, SecTileType>();
        for (int i = 0; i < Enum.GetValues(typeof(TileType)).Length - 1; i++)
        {
            prevSecTileTypes.Add((TileType) i, ContentManager.ValidTileTypes[(TileType) i][0]);
        }

        prevContentType = (ContentType) 0;
        prevTileType = (TileType) 0;

        // select the first tile and the first content types
        selectedContent = prevSecContentTypes.GetEntry(prevContentType);
        selectedTile = prevSecTileTypes.GetEntry(prevTileType);

        SetSelectedObject(selectedTile.Value);
    }

    public void Update()
    {
        if (UberManager.Instance.DoingSetup) return;

        HandleKeyboard();
        HandleMouse();
    }

    private void HandleKeyboard()
    {
        HandlePrimaryTypeSwitch();
        HandlePlacementSwitch();
        HandeToolSwitch();
    }

    private void HandlePrimaryTypeSwitch()
    {
        int pressedNumber = NumberKeyPressed();

        // if a number is pressed switch between primary types
        if (pressedNumber >= 0)
        {
            // is there a primary type matching this number for the current placement type?
            bool isValidType = placableType == PlacableType.Content
                ? pressedNumber < Enum.GetValues(typeof(ContentType)).Length - 1
                : pressedNumber < Enum.GetValues(typeof(TileType)).Length - 1;

            if (isValidType)
            {
                //TODO: not a key bug
                if (placableType == PlacableType.Content)
                {
                    if (selectedContent.Key != (ContentType)pressedNumber)
                    {
                        SetSelectedObject(prevSecContentTypes[(ContentType)pressedNumber]);
                    }
                }
                else
                {
                    if (selectedTile.Key != (TileType)pressedNumber)
                    {
                        SetSelectedObject(prevSecTileTypes[(TileType)pressedNumber]);
                    }
                }
            }
            else
            {
                Debug.LogError("No valid type found");
            }
        }
    }

    private void HandlePlacementSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (placableType != PlacableType.Tile)
            {
                placableType = PlacableType.Tile;
                SetSelectedObject(prevSecTileTypes[prevTileType]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            if (placableType != PlacableType.Content)
            {
                placableType = PlacableType.Content;
                SetSelectedObject(prevSecContentTypes[prevContentType]);
            }
        }
    }

    private void HandeToolSwitch()
    {
        if (Input.GetKeyDown(KeyCode.G))
            ChangeToolType(ToolType.Fill);
        else if (Input.GetKeyDown(KeyCode.B))
            ChangeToolType(ToolType.Brush);
    }

    private int NumberKeyPressed()
    {
        int returnNumber = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            returnNumber = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            returnNumber = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            returnNumber = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            returnNumber = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            returnNumber = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            returnNumber = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            returnNumber = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            returnNumber = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            returnNumber = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0))
            returnNumber = 9;

        return returnNumber;
    }

    private void HandleMouse()
    {
        worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        coordinateMousePosition = GameManager.Instance.TileManager.GetGridPosition(worldMousePosition);

        UpdateObjectPreview();

        // left click is tool
        if (Input.GetMouseButton(0))
        {
            PencilClicked(coordinateMousePosition);
        }
        // right click is delete
        else if (Input.GetMouseButton(1))
        {
            DeleteClicked(coordinateMousePosition);
        }
    }

    private void ChangeToolType(ToolType newType)
    {
        if (newType == toolType) return;

        toolType = newType;
        Cursor.SetCursor(newType == ToolType.Brush ? pencilCursor : fillCursor, new Vector2(40, 40), CursorMode.Auto);
    }



    private void SetSelectedObject(SecTileType secondairyType)
    {
        TileType primaryType = ContentManager.GetPrimaryFromSecTile(secondairyType);
        KeyValuePair<TileType, SecTileType> newSelected = new KeyValuePair<TileType, SecTileType>(primaryType, secondairyType);

        selectedTile = newSelected;
        prevTileType = selectedTile.Key;
        prevSecTileTypes[selectedTile.Key] = selectedTile.Value;

        if (previewObject != null) Destroy(previewObject.gameObject);

        CreateObjectPreview(ContentManager.Instance.TilePrefabs[new KeyValuePair<TileType, SecTileType>(primaryType, secondairyType)]);
    }

    private void SetSelectedObject(SecContentType secondairyType)
    {
        ContentType primaryType = ContentManager.GetPrimaryFromSecContent(secondairyType);
        KeyValuePair<ContentType, SecContentType> newSelected = new KeyValuePair<ContentType, SecContentType>(primaryType, secondairyType);

        selectedContent = newSelected;
        prevContentType = selectedContent.Key;
        prevSecContentTypes[selectedContent.Key] = selectedContent.Value;

        if (previewObject != null) Destroy(previewObject.gameObject);

        CreateObjectPreview(ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>(primaryType, secondairyType)]);
    }

    private void CreateObjectPreview(GameObject prefab)
    {
        previewObject = Instantiate(prefab, new Vector3(worldMousePosition.x, worldMousePosition.y, 100), Quaternion.identity).transform;
        previewObject.GetComponent<SpriteRenderer>().color *= new Color(1, 1, 1, 0.5f);
    }

    private void UpdateObjectPreview()
    {
        if (previewObject != null)
        {
            Vector2 worldPos = GameManager.Instance.TileManager.GetWorldPosition(coordinateMousePosition);
            previewObject.position = new Vector3(worldPos.x, worldPos.y, -5);
        }
    }

    private void FillClicked()
    {
        
    }

    private void PencilClicked(Coordinate coord)
    {
        if (placableType == PlacableType.Tile) PlaceTile(coord);
        else PlaceContent(coord);
    }

    private void PlaceTile(Coordinate coord)
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if it's not already this tile type on this tile
        if (existingNode == null || existingNode.GetSecType() != selectedTile.Value)
        {
            //TODO: push to undo stack

            GameManager.Instance.TileManager.SetTileTypeDEVMODE(selectedTile.Value, coord);
        }
    }

    private void PlaceContent(Coordinate coord)
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node is null, there cannot be placed anything
        if (existingNode != null)
        {
            //TODO: push to undo stack

            // cannot place content on top of each other!
            if (existingNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = selectedContent.Key;
                s.secType = selectedContent.Value;
                s.position = coord;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
            }
        }
        else
        {
            Debug.LogError("You cannot place content on a non-existing tile.");
        }
    }

    private void DeleteClicked(Coordinate coord)
    {
        if(placableType == PlacableType.Tile) DeleteTile(coord);
        else DeleteContent(coord);
    }

    private void DeleteTile(Coordinate coord)
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node actually exists, delete it
        if (existingNode != null)
        {
            GameManager.Instance.TileManager.RemoveTileDEVMODE(coord);
        }
    }

    private void DeleteContent(Coordinate coord)
    {
        
    }
}
