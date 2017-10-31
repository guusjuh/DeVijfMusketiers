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

    [SerializeField] private bool developersMode;
    private bool selectedDevMode;
    public bool DevelopersMode { get { return selectedDevMode; } }

#if UNITY_EDITOR
    private LevelEditor levelEditor;
    public LevelEditor LevelEditor { get { return levelEditor;  } }
#endif

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

    private SoundManager soundManager = new SoundManager();
    public SoundManager SoundManager { get { return soundManager; } }

   //TODO: r/w from/to XML file
    private PlayerData playerData = new PlayerData();
    public PlayerData PlayerData { get { return playerData; } }

    private bool doingSetup = true;
    public bool DoingSetup { get { return doingSetup; } }

    private bool tutorial = true;
    public bool Tutorial { get { return tutorial; } }

    private TutorialManager tutorialManager;
    public TutorialManager TutorialManager { get { return tutorialManager; } }

    public GUIStyle myStyle;

    public void Awake() {
        doingSetup = true;

        Application.targetFrameRate = 60;

        contentManager.Initialize();
        contractManager.Initialize();
        uiManager.Initialize();
        soundManager.Initialize();

        selectedDevMode = developersMode;

#if UNITY_EDITOR
        if (developersMode) StartDevMode();
#endif
        if (!developersMode) StartGameMode();
    }

#if UNITY_EDITOR
    private void StartDevMode()
    {
        tutorial = false;

        levelEditor = gameObject.AddComponent<LevelEditor>();
        levelEditor.Initialize();

        stateManagers.Add(GameStates.InGame, new GameManager());

        GameManager.SetLevelInfo(-1, null);

        state = GameStates.InGame;
        stateManagers.Get(state).Start();

        GameManager.CameraManager.ResetDEVMODE();
    }
#endif

    private void StartGameMode()
    {
        tutorial = true;

        if (tutorial)
        {
            tutorialManager = new TutorialManager();
            tutorialManager.Initialize();
        }

        stateManagers.Add(GameStates.InGame, new GameManager());
        stateManagers.Add(GameStates.PostGame, new PostGameManager());
        stateManagers.Add(GameStates.LevelSelection, new LevelSelectionManager());
        stateManagers.Add(GameStates.PreGame, new PreGameManager());

        state = GameStates.LevelSelection;
        stateManagers.Get(state).Start();
    } 

    public void Update() {
        doingSetup = false;

        stateManagers.Get(state).Update();
        uiManager.UpdateUI();
    }

    public void GotoState(GameStates nextState) {
        stateManagers.Get(state).Clear();
        prevState = state;
        state = nextState;
        stateManagers.Get(state).Start();
    }

    public void EndTutorial() {
        tutorial = false;
        tutorialManager = null;
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