using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public int LevelID { get { return 1; } }

    private int rows;
    public int Rows { get { return rows; } }

    private int columns;
    public int Columns { get { return columns; } }

    private int dangerGrowRate;
    public int DangerGrowRate { get { return dangerGrowRate; } }

    private int dangerStartTurn;
    public int DangerStartTurn { get { return dangerStartTurn; } }

    private ToolType toolType = ToolType.Brush;

    private PlacableType placableType = PlacableType.Tile;
    private SelectedTypeData selectedData;

    private Vector3 worldMousePosition;
    private Coordinate coordinateMousePosition;
    Texture2D fillCursor;
    Texture2D pencilCursor;
    private Transform previewObject; // the preview stuck on the mouse

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

        selectedData = new SelectedTypeData();
        selectedData.Initialize();

        SetSelectedObject(selectedData.selectedTile.Value);
    }

    public void AdjustSize(Vector2 newSize)
    {
        if (rows == (int) newSize.x && columns == (int) newSize.y) return;

        rows = (int)newSize.x;
        columns = (int)newSize.y;

        GameManager.Instance.TileManager.AdjustGridSizeDEVMODE();
    }

    public void AdjustDangerGrowRate(int newValue)
    {
        dangerGrowRate = newValue;
    }

    public void AdjustDangerStartTurn(int newValue)
    {
        dangerStartTurn = newValue;
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
                if (placableType == PlacableType.Content)
                {
                    if (selectedData.selectedContent.Key != (ContentType)pressedNumber)
                    {
                        SetSelectedObject(selectedData.prevSecContents[(ContentType)pressedNumber]);
                    }
                }
                else
                {
                    if (selectedData.selectedTile.Key != (TileType)pressedNumber)
                    {
                        SetSelectedObject(selectedData.prevSecTiles[(TileType)pressedNumber]);
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
                SetSelectedObject(selectedData.prevSecTiles[selectedData.prevTile]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            if (placableType != PlacableType.Content)
            {
                placableType = PlacableType.Content;
                SetSelectedObject(selectedData.prevSecContents[selectedData.prevContent]);
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

        if (!ValidMousePosition(worldMousePosition, coordinateMousePosition))
        {
            if (previewObject.gameObject.activeInHierarchy) previewObject.gameObject.SetActive(false);
            return;
        }

        UpdateObjectPreview();

        // left click is tool
        if (Input.GetMouseButton(0))
        {
            if(toolType == ToolType.Brush) PencilClicked(coordinateMousePosition);
            if (toolType == ToolType.Fill)
            {
                if (placableType == PlacableType.Content)
                    FillClicked(coordinateMousePosition);
                else if (placableType == PlacableType.Tile)
                    FillClicked(coordinateMousePosition, GameManager.Instance.TileManager.GetNodeReference(coordinateMousePosition) == null? SecTileType.Unknown : GameManager.Instance.TileManager.GetNodeReference(coordinateMousePosition).GetSecType());
            }
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

        selectedData.selectedTile = newSelected;
        selectedData.prevTile = selectedData.selectedTile.Key;
        selectedData.prevSecTiles[selectedData.selectedTile.Key] = selectedData.selectedTile.Value;

        if (previewObject != null) Destroy(previewObject.gameObject);

        CreateObjectPreview(ContentManager.Instance.TilePrefabs[new KeyValuePair<TileType, SecTileType>(primaryType, secondairyType)]);
    }

    private void SetSelectedObject(SecContentType secondairyType)
    {
        ContentType primaryType = ContentManager.GetPrimaryFromSecContent(secondairyType);
        KeyValuePair<ContentType, SecContentType> newSelected = new KeyValuePair<ContentType, SecContentType>(primaryType, secondairyType);

        selectedData.selectedContent = newSelected;
        selectedData.prevContent = selectedData.selectedContent.Key;
        selectedData.prevSecContents[selectedData.selectedContent.Key] = selectedData.selectedContent.Value;

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
            if(!previewObject.gameObject.activeInHierarchy) previewObject.gameObject.SetActive(true);

            Vector2 worldPos = GameManager.Instance.TileManager.GetWorldPosition(coordinateMousePosition);
            previewObject.position = new Vector3(worldPos.x, worldPos.y, -5);
        }
    }

    private void FillClicked(Coordinate coord)
    {
        if (placableType != PlacableType.Content) return;

        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        bool alreadyContent = existingNode != null && existingNode.GetAmountOfContent() > 0;
        bool cannotPlaceContent = (existingNode != null && existingNode.GetType() == TileType.Dangerous) || existingNode == null;

        bool occupied = alreadyContent || cannotPlaceContent;

        if (!ValidPosition(coord) || occupied) return;

        PlaceContent(coord);

        Coordinate[] directions = GameManager.Instance.TileManager.Directions(coord);

        for (int i = 0; i < directions.Length; i++)
        {
            FillClicked(coord + directions[i]);
        }
    }

    private void FillClicked(Coordinate coord, SecTileType initialType)
    {
        if (placableType != PlacableType.Tile) return;

        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        bool alreadyThisTileType = existingNode != null && existingNode.GetSecType() == selectedData.selectedTile.Value;
        bool sameTypeAsInitial = true;

        if (initialType == SecTileType.Unknown && existingNode == null)
            sameTypeAsInitial = true;
        else if (existingNode != null && existingNode.GetSecType() == initialType)
            sameTypeAsInitial = true;
        else
            sameTypeAsInitial = false;

        if (!ValidPosition(coord) || alreadyThisTileType || !sameTypeAsInitial) return;

        PlaceTile(coord);

        Coordinate[] directions = GameManager.Instance.TileManager.Directions(coord);

        for (int i = 0; i < directions.Length; i++)
        {
            FillClicked(coord + directions[i], initialType);
        }
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
        if (existingNode == null || existingNode.GetSecType() != selectedData.selectedTile.Value)
        {
            //TODO: push to undo stack

            GameManager.Instance.TileManager.SetTileTypeDEVMODE(selectedData.selectedTile.Value, coord);
        }
    }

    private void PlaceContent(Coordinate coord)
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node is null, there cannot be placed anything
        if (existingNode != null && existingNode.GetType() != TileType.Dangerous)
        {
            //TODO: push to undo stack

            // cannot place content on top of each other!
            if (existingNode.GetAmountOfContent() == 0)
            {
                SpawnNode s = new SpawnNode();
                s.type = selectedData.selectedContent.Key;
                s.secType = selectedData.selectedContent.Value;
                s.position = coord;

                GameManager.Instance.LevelManager.SpawnObjectDEVMODE(s);
            }
        }
        else
        {
            if(existingNode == null) Debug.LogError("You cannot place content on a non-existing tile.");
            else if(existingNode.GetType() == TileType.Dangerous) Debug.LogError("You cannot place content on a dangerous tile.");
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
            if (existingNode.GetAmountOfContent() <= 0)
            {
                GameManager.Instance.TileManager.RemoveTileDEVMODE(coord);
            }
            else
            {
                Debug.LogError("Cannot remove a tile with content on it.");
            }
        }
    }

    private void DeleteContent(Coordinate coord)
    {
        TileNode existingNode = GameManager.Instance.TileManager.GetNodeReference(coord);

        // if the node actually exists, delete it
        if (existingNode != null && existingNode.GetAmountOfContent() > 0)
        {
            GameManager.Instance.TileManager.RemoveContentDEVMODE(existingNode);
        }
    }

    private bool ValidMousePosition(Vector2 worldPos, Coordinate coordPos)
    {
        Vector2 coordinatesWorldPos = GameManager.Instance.TileManager.GetWorldPosition(coordPos);

        if ((coordinatesWorldPos - worldPos).magnitude > GameManager.Instance.TileManager.HexagonScale)
            return false;

        return true;
    }

    private bool ValidPosition(Coordinate coord)
    {
        // if the coord is outside the field, return false
        if (coord.x < 0 || coord.x >= rows || coord.y < 0 || coord.y >= columns)
            return false;

        return true;
    }
}
