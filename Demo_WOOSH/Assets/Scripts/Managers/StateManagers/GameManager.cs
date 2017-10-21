using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManager : StateManager {
    public enum SpellType
    {
        Attack = 0,
        FrostBite,
        Fireball,
        Teleport
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

    private CameraManager cameraManager;
    public CameraManager CameraManager { get { return cameraManager; } }

    private TileManager tileManager = new TileManager();
    public TileManager TileManager { get { return tileManager; } }

    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    private List<Contract> selectedContracts;
    public List<Contract> SelectedContracts { get { return selectedContracts; } }

    protected override void Initialize()
    { 
        tileManager.Initialize();
        UIManager.Instance.RestartUI();
        levelManager.Initialize();
        cameraManager = Camera.main.gameObject.AddComponent<CameraManager>();
        cameraManager.Initialize();

        gameOn = true;
        won = false;
    }

    protected override void Restart()
    {
        tileManager.Restart();
        UIManager.Instance.RestartUI();
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

    // update is called every frame
    public override void Update()
    {
        if (!gameOn) return;

        UberManager.Instance.InputManager.CatchInput();
        cameraManager.UpdatePosition();
        levelManager.Update();
    }

    public void SetLevelInfo(int levelID, List<Contract> selectedContracts)
    {
        currentLevel = levelID;
        this.selectedContracts = selectedContracts;
    }
}
