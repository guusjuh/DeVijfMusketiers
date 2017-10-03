using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public enum SpellType
    {
        Attack = 0,
        Repair,
        Invisible,
        Push
    }

    // singleton
    private static GameManager instance = null;
    public static GameManager Instance {
        get {
            if (instance == null) instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return instance;
        }
    }

    private bool doingSetup = true;

    [SerializeField] private ContentManager contentManager;
    public ContentManager ContentManager { get { return contentManager; } }

    private LevelManager levelManager;
    public LevelManager LevelManager { get { return levelManager; } }

    private UIManager uiManager;
    public UIManager UiManager { get { return uiManager; } }

    private TileManager tileManager;
    public TileManager TileManager { get { return tileManager; } }

    private InputManager inputManager;
    public InputManager InputManager { get { return inputManager; } }

    //TODO: enum for layers!!

    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    //TODO
    // 1st contenttype becomes content (THIS spefic boss)
    // 2nd contenttype stays the same
    private Dictionary<TileManager.ContentType, List<TileManager.ContentType>> typesToEnter = new Dictionary<TileManager.ContentType, List<TileManager.ContentType>>();
    public Dictionary<TileManager.ContentType, List<TileManager.ContentType>> TypesToEnter { get { return typesToEnter; } }

    public void Awake()
    {
        doingSetup = true;
        InitGame();
    }

    private void InitGame()
    { 
        contentManager.Initialize();
        SetTypesToEnter();

        tileManager = new TileManager();
        tileManager.Initialize();

        uiManager = new UIManager();
        uiManager.Initialize();

        levelManager = new LevelManager();
        levelManager.Initialize();

        inputManager = new InputManager();

    }

    private void SetTypesToEnter()
    {
        List<TileManager.ContentType> bossList = new List<TileManager.ContentType>();
        bossList.Add(TileManager.ContentType.Barrel);
        bossList.Add(TileManager.ContentType.BrokenBarrel);
        bossList.Add(TileManager.ContentType.FlyingMonster);

        typesToEnter.Add(TileManager.ContentType.WalkingMonster, bossList);
    }

    // update is called every frame
    public void Update()
    {
        inputManager.CatchInput();
        levelManager.Update();
    }

    public void GameOver()
    {
        
    }

    public void SkipPlayerTurn()
    {
        levelManager.EndPlayerMove(1, true);
        uiManager.HideSpellButtons();
    }
}
