using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR  
using UnityEditor;
#endif

public class UberManager : MonoBehaviour { 

    public enum GameStates {
        LevelSelection = 0,
        Hub,
        PreGame,
        InGame,
        PostGame
    }

    // singleton
    private static UberManager instance = null;
    public static UberManager Instance {
        get {
            if (instance == null) instance = FindObjectOfType(typeof(UberManager)) as UberManager;
            return instance;
        }
    }

    private Dictionary<GameStates, StateManager> stateManagers = new Dictionary<GameStates, StateManager>();
    public LevelSelectionManager LevelSelectionManager { get { return (LevelSelectionManager)stateManagers.Get(GameStates.LevelSelection); } }
    public PreGameManager PreGameManager { get { return (PreGameManager)stateManagers.Get(GameStates.PreGame); } }
    public GameManager GameManager { get { return (GameManager)stateManagers.Get(GameStates.InGame); } }
    public PostGameManager PostGameManager { get { return (PostGameManager)stateManagers.Get(GameStates.PostGame); } }

    private GameStates prevState;
    private GameStates state;
    public GameStates PrevGameState { get { return prevState; } }
    public GameStates GameState { get { return state; } }

    [SerializeField] private ContentManager contentManager = new ContentManager();
    public ContentManager ContentManager { get { return contentManager; } }

    [SerializeField] private ContractManager contractManager = new ContractManager();
    public ContractManager ContractManager { get { return contractManager; } }

    private InputManager inputManager = new InputManager();
    public InputManager InputManager { get { return inputManager; } }

    private UIManager uiManager = new UIManager();
    public UIManager UiManager { get { return uiManager; } }

   //TODO: r/w from/to XML file
    private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData { get { return playerData; } }

    private bool doingSetup = true;

    public void Awake() {
        doingSetup = true;

        Application.targetFrameRate = 60;

        contentManager.Initialize();
        uiManager.Initialize();
        contractManager.Initialize();

        stateManagers.Add(GameStates.InGame, new GameManager());
        stateManagers.Add(GameStates.PostGame, new PostGameManager());
        stateManagers.Add(GameStates.LevelSelection, new LevelSelectionManager());
        stateManagers.Add(GameStates.PreGame, new PreGameManager());

        state = GameStates.LevelSelection;
        stateManagers.Get(state).Start();
    }

    public void Update() {
        stateManagers.Get(state).Update();
        uiManager.UpdateUI();
    }

    public void GotoState(GameStates nextState) {
        stateManagers.Get(state).Clear();
        prevState = state;
        state = nextState;
        stateManagers.Get(state).Start();
    }

    public static T PerformRandomRoll<T>(Dictionary<T, int> possibilities) 
    {
        // sum all probabilities 
        int summedProbability = 0;
        foreach (KeyValuePair<T, int> entry in possibilities) summedProbability += entry.Value;

        // perform a random roll to find a random humantype
        int counter = 0;
        int randomRoll = (int)UnityEngine.Random.Range(0, summedProbability);

        // find the human type matching the random roll
        foreach (KeyValuePair<T, int> entry in possibilities)
        {
            counter += entry.Value;
            if (randomRoll < counter)
            {
                return entry.Key;
            }
        }

        Debug.LogError("Random roll has failed.");
        return default(T);
    }
}

#if UNITY_EDITOR  
[CustomEditor(typeof(UberManager))]
// ^ This is the script we are making a custom editor for.
public class UberEditorScript : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Save level data")) {
            ContentManager.Instance.SaveAllInformation();
            Debug.Log("Level Data saved");
        }
    }
}
#endif 