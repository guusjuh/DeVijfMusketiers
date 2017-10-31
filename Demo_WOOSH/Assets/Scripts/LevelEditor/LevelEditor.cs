#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
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
        Fill,
        Brush,
        Eraser,
    }

    private const KeyCode FILL = KeyCode.Q;
    private const KeyCode PENCIL = KeyCode.W;
    private const KeyCode ERASER = KeyCode.E;
    private const KeyCode UNDO = KeyCode.Z;
    private const KeyCode REDO = KeyCode.Y;
    private const KeyCode SWITCH_TILE_CONTENT = KeyCode.Tab;

    public const string FILL_HK        = "Q \t- Fill";
    public const string PENCIL_HK      = "W \t- Pencil";
    public const string ERASER_HK      = "E \t- Eraser";
    public const string TOGGLE_HK      = "TAB \t- Toggle between content/tile";
    public const string SWITCH_PRIM_HK = "1-9 \t- Switch primary type";
    public const string UNDO_HK        = "Z \t- Undo last tool action";
    public const string REDO_HK        = "Y \t- Redo last tool action";

    private GridOverlay gridOverlay;

    // the current tool used
    private ToolType toolType = ToolType.Brush;
    public ToolType CurrentToolType { get { return toolType; } }

    // the current type that can be placed
    private PlacableType placableType = PlacableType.Tile;
    public PlacableType CurrentPlacableType { get { return placableType; } }

    // class to hold the selected data
    private SelectedTypeData selectedData;

    // direct getters to the selected content and tile
    public KeyValuePair<ContentType, SecContentType> SelectedContent { get { return selectedData.selectedContent; } }
    public KeyValuePair<TileType, SecTileType> SelectedTile { get { return selectedData.selectedTile; } }

    // mouse position
    private Vector3 worldMousePosition;
    private Coordinate coordinateMousePosition;

    // cursors
    private Vector2 cursorOffset = new Vector2(40, 40);
    private Texture2D fillCursor;
    private Texture2D pencilCursor;
    private Texture2D eraserCursor;

    public Texture2D[] CursorButtons = new Texture2D[3];

    // preview objects
    private GameObject highlightPreviewObject;
    private Transform previewObject; // the preview stuck on the mouse

    // linked list for undo / redo (linked list so we can also empty from the other side when count grows to high)
    private LinkedList<EditorCommand> undoActions = new LinkedList<EditorCommand>();
    private LinkedList<EditorCommand> redoActions = new LinkedList<EditorCommand>();
    private const int MAX_ACTIONS_TO_SAVE = 1000;

    // ---------------------------------------------------------------
    private LevelData levelData;

    public int Rows { get { return levelData.rows; } }
    public int Columns { get { return levelData.columns; } }
    public int LevelID { get { return levelData.id; } }
    public int DangerStartTurn { get { return levelData.dangerStartGrow; } }
    public SecTileTypeRow[] Grid { get { return levelData.grid; } }
    public List<SpawnNode> SpawnNodes { get { return levelData.spawnNodes; } }

    // ---------------------------------------------------------------

    private const string LEVEL_PATH = "Assets/Resources/Levels/";
    private const string PRE_FIX = "level";
    private const string FILE_EXTENSION = ".txt";

    public void Initialize()
    {
        levelData = new LevelData();
        levelData.id = NewID();  // TODO: read from files
        levelData.spawnNodes = new List<SpawnNode>();
        levelData.rows = 7;
        levelData.columns = 9;
        levelData.dangerStartGrow = 3;

        BuildNewLevelDataGrid();

        // setup grid overlay
        gridOverlay = UberManager.Instance.gameObject.AddComponent<GridOverlay>();
        gridOverlay.Initialize();

        // setup mouse
        worldMousePosition = Vector3.zero;
        coordinateMousePosition = Coordinate.zero;

        fillCursor = Resources.Load<Texture2D>("Sprites/LevelEditor/fill");
        pencilCursor = Resources.Load<Texture2D>("Sprites/LevelEditor/pencil");
        eraserCursor = Resources.Load<Texture2D>("Sprites/LevelEditor/eraser");

        CursorButtons[0] = Resources.Load<Texture2D>("Sprites/LevelEditor/fillBttn");
        CursorButtons[1] = Resources.Load<Texture2D>("Sprites/LevelEditor/pencilBttn");
        CursorButtons[2] = Resources.Load<Texture2D>("Sprites/LevelEditor/eraserBttn");

        highlightPreviewObject = Instantiate(Resources.Load<GameObject>("Prefabs/PreviewHighlight"));
        highlightPreviewObject.SetActive(false);

        // set intial tooltype
        ChangeToolType(ToolType.Brush);

        // set intial selected 
        placableType = PlacableType.Tile;

        selectedData = new SelectedTypeData();
        selectedData.Initialize();

        SetSelectedObject(selectedData.selectedTile.Value);
    }

    public void StartNew()
    {
        EmptyActionList();

        //clear grid
        GameManager.Instance.TileManager.ClearGridDEVMODE();

        //clear objects
        GameManager.Instance.LevelManager.Clear();

        levelData = null;
        levelData = new LevelData();
        levelData.id = NewID(); 
        levelData.spawnNodes = new List<SpawnNode>();
        levelData.rows = 7;
        levelData.columns = 9;
        levelData.dangerStartGrow = 3;

        BuildNewLevelDataGrid();

        GameManager.Instance.TileManager.Restart();
        GameManager.Instance.LevelManager.RestartDEVMODE();

        // set intial tooltype
        ChangeToolType(ToolType.Brush);

        // set intial selected 
        placableType = PlacableType.Tile;

        SetSelectedObject(selectedData.selectedTile.Value);
    }

    public void LoadLevel(int id)
    {
        EmptyActionList();

        //clear grid
        GameManager.Instance.TileManager.ClearGridDEVMODE();

        //clear objects
        GameManager.Instance.LevelManager.Clear();

        levelData = null;
        levelData = ContentManager.Instance.LevelData(id);

        GameManager.Instance.TileManager.Restart();
        GameManager.Instance.LevelManager.RestartDEVMODE();

        GameManager.Instance.TileManager.CreateGridDEVMODE(levelData.grid);

        GameManager.Instance.LevelManager.SpawnLevelDEVMODE(levelData.spawnNodes);

        GameManager.Instance.CameraManager.ResetDEVMODE();
    }

    public void Pause(bool gamePaused)
    {
        if (gamePaused)
        {                
            // reset to intial
            if (EditorUtility.DisplayDialog("Which level do you want to edit?",
                "Do you want to continue with the initial edited level or use this in-game level to edit?",
                "Initial Level", "In-game Level"))
            {
                // reset the grid to initial types
                for (int i = 0; i < levelData.rows; i++)
                {
                    for (int j = 0; j < levelData.columns; j++)
                    {
                        GameManager.Instance.TileManager.SetTileTypeDEVMODE(levelData.grid[i].row[j], new Coordinate(i, j));
                    }
                }

                // reset the objects that belong to the spawnnodes
                GameManager.Instance.LevelManager.ResetAllToInitDEVMODE(levelData.spawnNodes);
                GameManager.Instance.LevelManager.ResetTurnAmount();
            }
            // save current to level data
            else
            {
                SetInGameLevelAsLevelData();
            }

            ChangeToolType(toolType);
            if (ValidMousePosition(worldMousePosition, coordinateMousePosition))
            {
                highlightPreviewObject.SetActive(true);
            }
        }
        else
        {
            GameManager.Instance.LevelManager.StartGapTurn = DangerStartTurn;
            highlightPreviewObject.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void GameOver()
    {
        if (EditorUtility.DisplayDialog("Game is over!",
                   "This level isn't playable anymore. Press O.K. to return to the last saved level.",
                   "O.K."))
        {
            // reset the grid to initial types
            for (int i = 0; i < levelData.rows; i++)
            {
                for (int j = 0; j < levelData.columns; j++)
                {
                    GameManager.Instance.TileManager.SetTileTypeDEVMODE(levelData.grid[i].row[j], new Coordinate(i, j));
                }
            }

            // reset the objects that belong to the spawnnodes
            GameManager.Instance.LevelManager.ResetAllToInitDEVMODE(levelData.spawnNodes);

            ChangeToolType(toolType);
            if (ValidMousePosition(worldMousePosition, coordinateMousePosition))
            {
                highlightPreviewObject.SetActive(true);
            }

            GameManager.Instance.RestartDEVMODE();
        }
    }

    public bool CurrentLevelIsPausable()
    {
        if (GameManager.Instance.TileManager.NoMoreThanOneAtATileDEVMODE())
        {
            Debug.LogError("Current level is not pausable (More than one object on a tile)");

            return false;
        }

        return true;
    }

    public bool CurrentLevelIsPlayable()
    {
        if (!GameManager.Instance.TileManager.ValidGridDEVMODE())
        {
            Debug.LogError("Current level is not playable  (Not a Valid Grid)");

            return false;
        }
        if (levelData.spawnNodes.FindAll(s => s.type == ContentType.Human).Count <= 0)
        {
            Debug.LogError("Current level is not playable (Missing Human)");

            return false;
        }
        if (levelData.spawnNodes.FindAll(s => s.type == ContentType.Boss || s.type == ContentType.Minion).Count <= 0)
        {
            Debug.LogError("Current level is not playable (Missing Enemy)");
            return false;
        }

        return true;
    }

    private void EmptyActionList()
    {
        undoActions.Clear();
        undoActions = null;
        undoActions = new LinkedList<EditorCommand>();

        redoActions.Clear();
        redoActions = null;
        redoActions = new LinkedList<EditorCommand>();
    }

    // ---------- VARS THAT CAN BE CHANGED IN LEVEL EDITOR WINDOW -----------
    public void AdjustSize(Vector2 newSize)
    {
        if (levelData.rows == (int) newSize.x && levelData.columns == (int) newSize.y) return;

        levelData.rows = (int)newSize.x;
        levelData.columns = (int)newSize.y;

        GameManager.Instance.TileManager.AdjustGridSizeDEVMODE();
        GameManager.Instance.CameraManager.ResetDEVMODE();
        UIManager.Instance.InGameUI.InitializeTeleportButtons();
        BuildNewLevelDataGrid();

        for (int i = 0; i < levelData.spawnNodes.Count; i++)
        {
            if (levelData.spawnNodes[i].position.x > Rows || levelData.spawnNodes[i].position.y > Columns)
            {
                levelData.spawnNodes.RemoveAt(i);
                i--;
            }
        }

        EmptyActionList();
    }

    public void AdjustDangerStartTurn(int newValue)
    {
        if (newValue <= 0) return;
        levelData.dangerStartGrow = newValue;
    }
    // ----------------------------------------------------------------------

    public void Update()
    {
        if (UberManager.Instance.DoingSetup || !GameManager.Instance.Paused) return;

        HandleKeyboard();
        HandleMouse();
    }

    private void HandleKeyboard()
    {
        HandlePrimaryTypeSwitch();
        HandlePlacementSwitch();
        HandeToolSwitch();

        // undo
        if (Input.GetKeyDown(UNDO))
        {
            if (undoActions.Count <= 0)
            {
                Debug.Log("no actions to undo");
                return;
            }

            if (undoActions.Count > MAX_ACTIONS_TO_SAVE)
            {
                undoActions.RemoveFirst();
            }

            // execute undo
            undoActions.Last.Value.undoMethod();

            // add the last undo action to the redo stack
            redoActions.AddLast(undoActions.Last.Value);

            // remove the last undo action
            undoActions.RemoveLast();
            Debug.Log("undo"); 
        }
        // redo
        else if (Input.GetKeyDown(REDO))
        {
            if (redoActions.Count <= 0)
            {
                Debug.Log("no actions to redo");
                return;
            }

            if (redoActions.Count > MAX_ACTIONS_TO_SAVE)
            {
                redoActions.RemoveFirst();
            }

            // execute redo
            redoActions.Last.Value.redoMethod();

            // add the last redo action to the undo stack
            undoActions.AddLast(redoActions.Last.Value);

            // remove the last redo action
            redoActions.RemoveLast();

            Debug.Log("redo");
        }
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
            if (highlightPreviewObject.activeInHierarchy) highlightPreviewObject.SetActive(false);
            return;
        }

        UpdateObjectPreview();

        // left click is tool
        if (Input.GetMouseButtonDown(0))
            // happy 420!
        {
            if (toolType == ToolType.Brush)
            {
                PencilCommand newCommand = new PencilCommand(coordinateMousePosition);
                if (newCommand.Execute(coordinateMousePosition)) undoActions.AddLast(newCommand);
            }
            if (toolType == ToolType.Fill)
            {
                FillCommand newCommand = new FillCommand(coordinateMousePosition);
                if (newCommand.Execute(coordinateMousePosition)) undoActions.AddLast(newCommand);
            }
            if (toolType == ToolType.Eraser)
            {
                DeleteCommand newCommand = new DeleteCommand(coordinateMousePosition);
                if (newCommand.Execute(coordinateMousePosition)) undoActions.AddLast(newCommand);
            }
        }
        // right click is delete
        else if (Input.GetMouseButtonDown(1))
        {
            DeleteCommand newCommand = new DeleteCommand(coordinateMousePosition);
            if (newCommand.Execute(coordinateMousePosition)) undoActions.AddLast(newCommand);
        }
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
        if (Input.GetKeyDown(SWITCH_TILE_CONTENT))
        {
            if (placableType != PlacableType.Tile)
            {
                placableType = PlacableType.Tile;
                SetSelectedObject(selectedData.prevSecTiles[selectedData.prevTile]);
            }else if (placableType != PlacableType.Content)
            {
                placableType = PlacableType.Content;
                SetSelectedObject(selectedData.prevSecContents[selectedData.prevContent]);
            }
        }
    }

    private void HandeToolSwitch()
    {
        if (Input.GetKeyDown(FILL))
            ChangeToolType(ToolType.Fill);
        else if (Input.GetKeyDown(PENCIL))
            ChangeToolType(ToolType.Brush);
        else if (Input.GetKeyDown(ERASER))
            ChangeToolType(ToolType.Eraser);
    }

    public void ChangeToolType(ToolType newType)
    {
        toolType = newType;
        Texture2D newCursor = pencilCursor;
        if (newType == ToolType.Fill) newCursor = fillCursor;
        if (newType == ToolType.Eraser) newCursor = eraserCursor;
        Cursor.SetCursor(newCursor, cursorOffset, CursorMode.Auto);
    }

    public void SetSelectedObject(SecTileType secondairyType)
    {
        TileType primaryType = ContentManager.GetPrimaryFromSecTile(secondairyType);
        KeyValuePair<TileType, SecTileType> newSelected = new KeyValuePair<TileType, SecTileType>(primaryType, secondairyType);

        placableType = PlacableType.Tile;
        selectedData.selectedTile = newSelected;
        selectedData.prevTile = selectedData.selectedTile.Key;
        selectedData.prevSecTiles[selectedData.selectedTile.Key] = selectedData.selectedTile.Value;

        if (previewObject != null)
        {
            Destroy(previewObject.gameObject);
            highlightPreviewObject.SetActive(false);
        }

        CreateObjectPreview(ContentManager.Instance.TilePrefabs[new KeyValuePair<TileType, SecTileType>(primaryType, secondairyType)]);
    }

    public void SetSelectedObject(SecContentType secondairyType)
    {
        ContentType primaryType = ContentManager.GetPrimaryFromSecContent(secondairyType);
        KeyValuePair<ContentType, SecContentType> newSelected = new KeyValuePair<ContentType, SecContentType>(primaryType, secondairyType);

        placableType = PlacableType.Content;
        selectedData.selectedContent = newSelected;
        selectedData.prevContent = selectedData.selectedContent.Key;
        selectedData.prevSecContents[selectedData.selectedContent.Key] = selectedData.selectedContent.Value;

        if (previewObject != null)
        {
            Destroy(previewObject.gameObject);
            highlightPreviewObject.SetActive(false);
        }

        CreateObjectPreview(ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>(primaryType, secondairyType)]);
    }

    private void CreateObjectPreview(GameObject prefab)
    {
        if(ValidMousePosition(worldMousePosition, coordinateMousePosition)) highlightPreviewObject.SetActive(true);

        previewObject = Instantiate(prefab, Vector3.zero, Quaternion.identity).transform;
        previewObject.GetComponent<SpriteRenderer>().color *= new Color(1, 1, 1, 0.5f);
        previewObject.parent = highlightPreviewObject.transform;
        previewObject.localPosition = Vector3.zero;
    }

    private void UpdateObjectPreview()
    {
        if (previewObject != null)
        {
            if (toolType != ToolType.Eraser)
            {
                if (!previewObject.gameObject.activeInHierarchy) previewObject.gameObject.SetActive(true);
                if (!highlightPreviewObject.activeInHierarchy) highlightPreviewObject.SetActive(true);
            }
            else
            {
                if (previewObject.gameObject.activeInHierarchy) previewObject.gameObject.SetActive(false);
            }

            Vector2 worldPos = GameManager.Instance.TileManager.GetWorldPosition(coordinateMousePosition);
            highlightPreviewObject.transform.position = new Vector3(worldPos.x, worldPos.y, -5);
        }
    }

    private bool ValidMousePosition(Vector2 worldPos, Coordinate coordPos)
    {
        if (UberManager.Instance.DoingSetup) return false;

        Vector2 coordinatesWorldPos = GameManager.Instance.TileManager.GetWorldPosition(coordPos);

        if ((coordinatesWorldPos - worldPos).magnitude > GameManager.Instance.TileManager.HexagonScale)
            return false;

        return true;
    }

    public bool ValidPosition(Coordinate coord)
    {
        // if the coord is outside the field, return false
        if (coord.x < 0 || coord.x >= Rows || coord.y < 0 || coord.y >= Columns)
            return false;

        return true;
    }

    // ----- UPDATING THE LEVEL DATA --------------

    private void BuildNewLevelDataGrid()
    {
        levelData.grid = null;

        levelData.grid = new SecTileTypeRow[levelData.rows];
        for (int i = 0; i < levelData.rows; i++)
        {
            levelData.grid[i] = new SecTileTypeRow();
            levelData.grid[i].row = new SecTileType[levelData.columns];
        }
    }

    private void SetInGameLevelAsLevelData()
    {
        EmptyActionList();

        // clear the current spawnnodes
        levelData.spawnNodes.Clear();
        levelData.spawnNodes = null;

        // initialize new spawnnodes
        levelData.spawnNodes = new List<SpawnNode>();

        // create new spawn node all humans
        for (int i = 0; i < GameManager.Instance.LevelManager.Humans.Count; i++)
        {
            levelData.spawnNodes.Add(GenerateSpawnNodeFromWorldObject(GameManager.Instance.LevelManager.Humans[i]));
        }

        // create new spawn node all enemies
        for (int i = 0; i < GameManager.Instance.LevelManager.Enemies.Count; i++)
        {
            levelData.spawnNodes.Add(GenerateSpawnNodeFromWorldObject(GameManager.Instance.LevelManager.Enemies[i]));
        }

        // create new spawn node all barrels
        for (int i = 0; i < GameManager.Instance.LevelManager.Barrels.Count; i++)
        {
            levelData.spawnNodes.Add(GenerateSpawnNodeFromWorldObject(GameManager.Instance.LevelManager.Barrels[i]));
        }

        // create new spawn node all shrines
        for (int i = 0; i < GameManager.Instance.LevelManager.Shrines.Count; i++)
        {
            levelData.spawnNodes.Add(GenerateSpawnNodeFromWorldObject(GameManager.Instance.LevelManager.Shrines[i]));
        }

        // clear and init the current grid
        BuildNewLevelDataGrid();

        // set all new types
        for (int i = 0; i < levelData.rows; i++)
        {
            for (int j = 0; j < levelData.columns; j++)
            {
                levelData.grid[i].row[j] =
                    GameManager.Instance.TileManager.GetNodeReference(new Coordinate(i, j)).GetSecType();
            }
        }
    }

    private SpawnNode GenerateSpawnNodeFromWorldObject(WorldObject worldObject)
    {
        SpawnNode newNode = new SpawnNode();
        newNode.type = ContentManager.GetPrimaryFromSecContent(worldObject.Type);
        newNode.secType = worldObject.Type;
        newNode.position = worldObject.GridPosition;

        return newNode;
    }

    private int NewID()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Levels");
        FileInfo[] allFiles = dir.GetFiles("*.txt");
        List<int> validFiles = new List<int>();

        foreach (FileInfo f in allFiles)
        {
            Match match = Regex.Match(f.Name, @"\d+");//"[0-9]*");

            validFiles.Add(int.Parse(match.Value));
        }
        return validFiles.Count;
    }

    public void SaveCurrentLevel()
    {
        FileStream fs = new FileStream(LEVEL_PATH + PRE_FIX + LevelID + FILE_EXTENSION, FileMode.Create);

        XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
        serializer.Serialize(fs, levelData);

        fs.Close();
    }
}
#endif
