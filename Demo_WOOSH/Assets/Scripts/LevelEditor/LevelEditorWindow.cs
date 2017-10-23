using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelEditor levelEditorRef;

    private bool showProperties;

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
                levelEditorRef.AdjustSize = EditorGUILayout.Vector2Field("Level Size: ", new Vector2(levelEditorRef.Rows, levelEditorRef.Columns));
                GUILayout.EndHorizontal();

                // danger grow rate
                GUILayout.BeginHorizontal();
                levelEditorRef.AdjustDangerGrowRate = EditorGUILayout.IntField("Danger grow speed: ", levelEditorRef.AdjustDangerGrowRate);
                GUILayout.EndHorizontal();

                // danger grow start turn 
                GUILayout.BeginHorizontal();
                levelEditorRef.AdjustDangerStartTurn = EditorGUILayout.IntField("Danger start turn: ", levelEditorRef.AdjustDangerStartTurn);
                GUILayout.EndHorizontal();

                // end box layout
                GUILayout.EndVertical();
            }
            // ---------------------------------------------

            // toggle content vs tile edit mode
            // toggle brush vs fill mode

            /*GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();*/


        }
    }
}
