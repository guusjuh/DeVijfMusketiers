using System.Collections.Generic;
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

    private int rows = 3;
    private float textureWidth;

    private Vector2 scrollPos = Vector2.zero;

    int selectedTool = 0;


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
            if(levelEditorRef != null) textureWidth = ContentManager.Instance.TileTextures[TileType.Dangerous][0].width;
            Repaint();
            return;
        }
    }

    void OnGUI()
    {
        if (levelEditorRef != null)
        {
            using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Width(position.width - 5),
                    GUILayout.Height(position.height - 5)))
            {
                scrollPos = scrollViewScope.scrollPosition;

                // -------------- TOOLS ------------------------

                selectedTool = GUILayout.SelectionGrid((int)levelEditorRef.CurrentToolType, levelEditorRef.CursorButtons, 2, GUILayout.Width(60), GUILayout.Height(30));
                if (selectedTool != (int) levelEditorRef.CurrentToolType) levelEditorRef.ChangeToolType((LevelEditor.ToolType)selectedTool);

                // ---------------------------------------------

                // -------------- PROPERTIES -------------------
                showProperties = EditorGUILayout.Foldout(showProperties, "Properties");
                if (showProperties)
                {
                    ShowProperties();
                }
                // ---------------------------------------------

                // buttons for content vs tile edit mode
                // buttons for brush vs fill mode

                // ------------ TILES -------------------------
                showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
                if (showTiles)
                {

                    // minus 5 for the scrollbars
                    GUILayout.BeginVertical("box");

                    showNormals = EditorGUILayout.Foldout(showNormals, "Normal Tiles");
                    if (showNormals)
                    {
                        ShowTypes(TileType.Normal);
                    }

                    showDangerous = EditorGUILayout.Foldout(showDangerous, "Dangerous Tile");
                    if (showDangerous)
                    {
                        ShowTypes(TileType.Dangerous);
                    }

                    GUILayout.EndVertical();
                }
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
                // ---------------------------------------------

            }
        }
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

    private void ShowProperties()
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
        levelEditorRef.AdjustSize(EditorGUILayout.Vector2Field("Level Size: ", new Vector2(levelEditorRef.Rows, levelEditorRef.Columns)));
        GUILayout.EndHorizontal();

        // danger grow rate
        GUILayout.BeginHorizontal();
        levelEditorRef.AdjustDangerGrowRate(EditorGUILayout.IntField("Danger grow speed: ", levelEditorRef.DangerGrowRate));
        GUILayout.EndHorizontal();

        // danger grow start turn 
        GUILayout.BeginHorizontal();
        levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
        GUILayout.EndHorizontal();

        // end box layout
        GUILayout.EndVertical();
    }
}
