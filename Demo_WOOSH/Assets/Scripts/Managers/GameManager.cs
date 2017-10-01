using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    // singleton
    private static GameManager instance = null;
    public static GameManager Instance {
        get {
            if (instance == null) instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return instance;
        }
    }

    private bool doingSetup = true;

    private bool playersTurn = false;
    private bool othersTurn = false;
    private int amountOfTurns = 0;

    [SerializeField] private float turnDelay = 0.1f;

    private ContentManager contentManager;
    public ContentManager ContentManager { get { return contentManager; } }

    private LevelManager levelManager;
    public LevelManager LevelManager { get { return levelManager; } }

    private UIManager uiManager;
    public UIManager UiManager { get { return uiManager; } }

    private TileManager tileManager;
    public TileManager TileManager { get { return tileManager; } }

    public void Awake()
    {
        doingSetup = true;
        InitGame();
    }

    private void InitGame()
    {
        contentManager = new ContentManager();
        contentManager.Initialize();

        tileManager = new TileManager();
        tileManager.Initialize(5, 5);

        // start with player turn
        playersTurn = true;
        BeginPlayerTurn();
        othersTurn = false;
    }

    public void GameOver()
    {
        
    }

    public void BeginPlayerTurn()
    {
        
    }

    public void EndPlayerTurn()
    {
        
    }
}
