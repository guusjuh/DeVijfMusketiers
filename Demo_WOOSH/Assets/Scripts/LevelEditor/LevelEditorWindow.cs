#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor levelEditorRef;

    private bool showProperties;
    private bool showTiles;
    private bool showNormals, showDangerous;
    private bool showContent;
    private bool showBosses, showMinions, showEnvironment, showHumans;
    private bool showHotKeys;

    private int rows = 3;

    private Vector2 scrollPos = Vector2.zero;

    int selectedTool = 0;

    private Vector2 levelSize = new Vector2(0,0);

    public void SetLevelSize(Vector2 newSize)
    {
        levelSize = newSize;
    }

    private int levelToLoadID = 1;

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditorWindow), false, "Level Editor");
    }

    void Update()
    {
        if (levelEditorRef == null)
        {
            levelEditorRef = UberManager.Instance.LevelEditor;
            if (levelEditorRef != null)
            {
                levelSize = new Vector2(levelEditorRef.Rows, levelEditorRef.Columns);
            }
        }
        Repaint();
    }

    void OnGUI()
    {
        if (levelEditorRef != null && GameManager.Instance.Paused)
        {
            using (
                var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Width(position.width - 5),
                    GUILayout.Height(position.height - 5)))
            {
                scrollPos = scrollViewScope.scrollPosition;

                // -------------- SAVE, LOAD, NEW ------------------------

                NewLevel();
                SeperationLine();

                SaveLevel();
                SeperationLine();

                LoadLevel();
                SeperationLine();

                // -------------- TOOLS ------------------------
                selectedTool = GUILayout.SelectionGrid((int)levelEditorRef.CurrentToolType, levelEditorRef.CursorButtons, 3, GUILayout.Width(90), GUILayout.Height(30));

                if (selectedTool != (int)levelEditorRef.CurrentToolType) levelEditorRef.ChangeToolType((LevelEditor.ToolType) selectedTool);

                SeperationLine();
                // ---------------------------------------------

                // -------------- PROPERTIES -------------------
                showProperties = EditorGUILayout.Foldout(showProperties, "Properties");
                if (showProperties)
                {
                    Properties();
                }

                SeperationLine();
                // ---------------------------------------------

                // ------------ TILES -------------------------
                showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
                if (showTiles)
                {

                    // minus 5 for the scrollbars
                    GUILayout.BeginVertical("box");

                    showNormals = EditorGUILayout.Foldout(showNormals, "Normal Tiles");
                    if (showNormals) ShowTypes(TileType.Normal);

                    showDangerous = EditorGUILayout.Foldout(showDangerous, "Dangerous Tile");
                    if (showDangerous) ShowTypes(TileType.Dangerous);

                    GUILayout.EndVertical();
                }

                SeperationLine();
                // ---------------------------------------------

                // ------------ CONTENT -------------------------
                showContent = EditorGUILayout.Foldout(showContent, "Content");
                if (showContent)
                {
                    GUILayout.BeginVertical("box");

                    showBosses = EditorGUILayout.Foldout(showBosses, "Bosses");
                    if (showBosses) ShowTypes(ContentType.Boss);

                    showMinions = EditorGUILayout.Foldout(showMinions, "Minions");
                    if (showMinions) ShowTypes(ContentType.Minion);

                    showEnvironment = EditorGUILayout.Foldout(showEnvironment, "Environment");
                    if (showEnvironment) ShowTypes(ContentType.Environment);

                    showHumans = EditorGUILayout.Foldout(showHumans, "Humans");
                    if (showHumans) ShowTypes(ContentType.Human);

                    GUILayout.EndVertical();
                }

                SeperationLine();
                // ---------------------------------------------

                showHotKeys = EditorGUILayout.Foldout(showHotKeys, "Hotkeys cheatsheet");
                if (showHotKeys)
                {
                    GUILayout.BeginVertical("box");

                    EditorGUILayout.TextArea(LevelEditor.FILL_HK + "\n" +
                                             LevelEditor.PENCIL_HK + "\n" +
                                             LevelEditor.ERASER_HK + "\n" +
                                             LevelEditor.UNDO_HK + "\n" +
                                             LevelEditor.REDO_HK + "\n" +
                                             LevelEditor.TOGGLE_HK + "\n" +
                                             LevelEditor.SWITCH_PRIM_HK);

                    GUILayout.EndVertical();
                }
                // ------------------------------------------------


            }
        }
    }

    private void SeperationLine()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    private void NewLevel()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New Level"))
        {
            if (EditorUtility.DisplayDialog("Start new level?",
                "By starting a new level you will discard all changes. Are you sure?",
                "Start New Level", "Go Back"))
            {
                levelEditorRef.StartNew();
            }
        }
        GUILayout.EndHorizontal();
    }

    private void SaveLevel()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Level"))
        {
            if (!levelEditorRef.CurrentLevelIsPlayable()) return;
            levelEditorRef.SaveCurrentLevel();

            if (!EditorUtility.DisplayDialog("Continue editing this level?",
                "You saved the current level. Would you like to continue or start a new level?",
                "Continue With current", "Start New Level"))
            {
                levelEditorRef.StartNew();
            }
        }
        GUILayout.EndHorizontal();
    }

    private void LoadLevel()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("ID to load: ");
        levelToLoadID = EditorGUILayout.IntField(levelToLoadID);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Level"))
        {
            ContentManager.Instance.ReadLevelData();

            // cannot load a level that isn't loaded by the contentmanager
            if (ContentManager.Instance.LevelData(levelToLoadID) == null)
            {
                Debug.LogError("Cannot load this level (doesn't exist)");
                return;
            }

            levelEditorRef.LoadLevel(levelToLoadID);
        }
        GUILayout.EndHorizontal();
    }

    private void ShowTypes(ContentType type)
    {
        bool selectedThisPrim = levelEditorRef.CurrentPlacableType == LevelEditor.PlacableType.Content && levelEditorRef.SelectedContent.Key == type;
        int selectionIndex = selectedThisPrim ? ContentManager.ValidContentTypes[levelEditorRef.SelectedContent.Key].FindIndex(t => t == levelEditorRef.SelectedContent.Value) : -1;

        GUILayout.BeginHorizontal();
        selectionIndex = GUILayout.SelectionGrid(selectionIndex, ContentManager.Instance.ContentTextures[type], rows, UberManager.Instance.myStyle);
        GUILayout.EndHorizontal();

        bool somethingSelected = selectionIndex >= 0;
        bool otherSecSamePrim = levelEditorRef.SelectedContent.Value != (SecContentType)selectionIndex &&
                                levelEditorRef.SelectedContent.Key == type;
        bool otherPrim = levelEditorRef.SelectedContent.Key != type || levelEditorRef.CurrentPlacableType != LevelEditor.PlacableType.Content;

        if (somethingSelected && (otherSecSamePrim || otherPrim))
        {
            levelEditorRef.SetSelectedObject(ContentManager.ValidContentTypes[type][selectionIndex]);
        }
    }

    private void ShowTypes(TileType type)
    {
        bool selectedThisPrim = levelEditorRef.CurrentPlacableType == LevelEditor.PlacableType.Tile && levelEditorRef.SelectedTile.Key == type;
        int selectionIndex = selectedThisPrim ? ContentManager.ValidTileTypes[levelEditorRef.SelectedTile.Key].FindIndex(t => t == levelEditorRef.SelectedTile.Value) : -1;

        GUILayout.BeginHorizontal();
        selectionIndex = GUILayout.SelectionGrid(selectionIndex, ContentManager.Instance.TileTextures[type], rows, UberManager.Instance.myStyle);
        GUILayout.EndHorizontal();

        bool somethingSelected = selectionIndex >= 0;
        bool otherSecSamePrim = levelEditorRef.SelectedTile.Value != (SecTileType) selectionIndex &&
                                levelEditorRef.SelectedTile.Key == type;
        bool otherPrim = levelEditorRef.SelectedTile.Key != type || levelEditorRef.CurrentPlacableType != LevelEditor.PlacableType.Tile;

        if (somethingSelected && (otherSecSamePrim || otherPrim))
        {
            levelEditorRef.SetSelectedObject(ContentManager.ValidTileTypes[type][selectionIndex]);
        }
    }

    private void Properties()
    {
        // create a box layout
        GUILayout.BeginVertical("box");

        // level id cannot be changed
        GUILayout.BeginHorizontal();
        GUILayout.Label("Level ID: ");
        GUI.enabled = false;
        GUILayout.TextField(levelEditorRef.LevelID.ToString());
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        // rows and cols
        GUILayout.BeginHorizontal();
        levelSize = EditorGUILayout.Vector2Field("Level Size: ", levelSize);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Apply level size"))
        {
            levelEditorRef.AdjustSize(levelSize);
        }

        // danger grow start turn 
        GUILayout.BeginHorizontal();
        levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
        GUILayout.EndHorizontal();

        // end box layout
        GUILayout.EndVertical();
    }
}
#endif