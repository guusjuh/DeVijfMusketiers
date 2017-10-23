using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor levelEditorRef;

    private bool showProperties;

    private bool showTiles;
    private bool showNormals;
    private bool showDangerous;

    private bool showContent;
    private bool showBosses;
    private bool showMinions;
    private bool showEnvironment;
    private bool showHumans;

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
            Repaint();
            return;
        }
    }

    void OnGUI()
    {
        if (levelEditorRef != null)
        {
            // -------------- PROPERTIES -------------------
            showProperties = EditorGUILayout.Foldout(showProperties, "Properties");
            if (showProperties)
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
            // ---------------------------------------------

            // buttons for content vs tile edit mode
            // buttons for brush vs fill mode

            // types
            showTiles = EditorGUILayout.Foldout(showTiles, "Tiles");
            if (showTiles)
            {
                GUILayout.BeginVertical("box");

                showNormals = EditorGUILayout.Foldout(showNormals, "Normal Tiles");
                if (showNormals)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();

                }

                showDangerous = EditorGUILayout.Foldout(showDangerous, "Dangerous Tile");
                if (showDangerous)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

            }

            showContent = EditorGUILayout.Foldout(showContent, "Content");
            if (showContent)
            {
                GUILayout.BeginVertical("box");

                showBosses = EditorGUILayout.Foldout(showBosses, "Bosses");
                if (showDangerous)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();
                }

                showMinions = EditorGUILayout.Foldout(showMinions, "Minions");
                if (showDangerous)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();
                }

                showEnvironment = EditorGUILayout.Foldout(showEnvironment, "Environment");
                if (showDangerous)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();
                }

                showHumans = EditorGUILayout.Foldout(showHumans, "Humans");
                if (showDangerous)
                {
                    GUILayout.BeginHorizontal();
                    levelEditorRef.AdjustDangerStartTurn(EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.DangerStartTurn));
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

            }
        }
    }
}
