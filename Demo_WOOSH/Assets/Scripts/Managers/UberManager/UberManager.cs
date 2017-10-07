using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UberManager : MonoBehaviour {

    public enum GameStates
    {
        LevelSelection = 0,
        Hub,
        PreGame,
        InGame
    }

    // singleton
    private static UberManager instance = null;
    public static UberManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType(typeof(UberManager)) as UberManager;
            return instance;
        }
    }

    private Dictionary<GameStates, StateManager> stateManagers = new Dictionary<GameStates, StateManager>();
    public GameManager GameManager { get { return (GameManager)stateManagers.Get(GameStates.InGame); } }

    [SerializeField] private ContentManager contentManager = new ContentManager();
    public ContentManager ContentManager { get { return contentManager; } }

    private InputManager inputManager = new InputManager();
    public InputManager InputManager { get { return inputManager; } }

    private UIManager uiManager = new UIManager();
    public UIManager UiManager { get { return uiManager; } }

    private GameStates prevState;
    private GameStates state;
    public GameStates GameState { get { return state; } }

    private bool doingSetup = true;

    public void Awake()
    {
        doingSetup = true;

        Application.targetFrameRate = 60;

        contentManager.Initialize();
        uiManager.Initialize();

        stateManagers.Add(GameStates.InGame, new GameManager());
        
        state = GameStates.InGame;
        stateManagers.Get(state).Initialize();
    }

    public void Update()
    {
        stateManagers.Get(state).Update();
    }

    public void GotoState(GameStates nextState)
    {
        stateManagers.Get(state).Clear();
        prevState = state;
        state = nextState;
        stateManagers.Get(state).Restart();
    }
}
