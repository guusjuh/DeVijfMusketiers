using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManager : StateManager {
    public enum SpellType
    {
        Attack = 0
    }

    // singleton
    private static GameManager instance = null;
    public static GameManager Instance {
        get {
            if (instance == null) instance = UberManager.Instance.GameManager;
            return instance;
        }
    }

    private bool gameOn = false;
    public bool GameOn { get { return gameOn; } }
    private bool won = false;
    public bool Won { get { return won; } }

    private LevelManager levelManager = new LevelManager();
    public LevelManager LevelManager { get { return levelManager; } }

    //TODO: make possible to work for ubermanager
    private CameraManager cameraManager;
    public CameraManager CameraManager { get { return cameraManager; } }

    private TileManager tileManager = new TileManager();
    public TileManager TileManager { get { return tileManager; } }

    //TODO: enum for layers!!

    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    private List<Contract> selectedContracts;
    public List<Contract> SelectedContracts { get { return selectedContracts; } }

    //TODO
    // 1st contenttype becomes content (THIS spefic boss)
    // 2nd contenttype stays the same
    private Dictionary<TileManager.ContentType, List<TileManager.ContentType>> typesToEnter = new Dictionary<TileManager.ContentType, List<TileManager.ContentType>>();
    public Dictionary<TileManager.ContentType, List<TileManager.ContentType>> TypesToEnter { get { return typesToEnter; } }

    protected override void Initialize()
    { 
        SetTypesToEnter();

        tileManager.Initialize();
        UIManager.Instance.RestartUI();//InGameUI.Start();
        levelManager.Initialize();
        cameraManager = Camera.main.gameObject.AddComponent<CameraManager>();
        cameraManager.Initialize();

        gameOn = true;
        won = false;
    }

    protected override void Restart()
    {
        tileManager.Restart();
        UIManager.Instance.RestartUI();//InGameUI.Start();
        levelManager.Restart();
        cameraManager.Initialize();

        gameOn = true;
        won = false;
    }

    public override void Clear()
    {
        gameOn = false;

        // call all clear methods
        levelManager.Clear();
        UIManager.Instance.ClearUI();
        tileManager.ClearGrid();

        // Activate the garbage collector so we start clean.
        GC.Collect();

        // Unload all unused assets to clear memory.
        Resources.UnloadUnusedAssets();
    }

    public void GameOver()
    {
        gameOn = false;

        if (LevelManager.Humans.Count > 0) won = true;

        // Switch game state
        UberManager.Instance.GotoState(UberManager.GameStates.PostGame);
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
    public override void Update()
    {
        if (!gameOn) return;

        UberManager.Instance.InputManager.CatchInput();
        cameraManager.UpdatePosition();
        levelManager.Update();
    }

    public void SkipPlayerTurn()
    {
        levelManager.EndPlayerMove(1, true);
        UIManager.Instance.InGameUI.HideSpellButtons();
        UIManager.Instance.InGameUI.ActivatePushButtons(false);
    }

    public void SetLevelInfo(int levelID, List<Contract> selectedContracts)
    {
        currentLevel = levelID - 1;
        this.selectedContracts = selectedContracts;
    }
}
