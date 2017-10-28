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

    private bool pause = false;
    public bool Paused { get {return pause; } private set { pause = value; } }

    private bool gameOn = false;

    public bool GameOn { get{ return gameOn; } }

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
        if (UberManager.Instance.DevelopersMode) pause = true;

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
        if (UberManager.Instance.DevelopersMode) pause = true;

        tileManager.Restart();
        UIManager.Instance.RestartUI();
        levelManager.Restart();
        cameraManager.Initialize();

        gameOn = true;
        won = false;
    }

    public override void Clear()
    {
        pause = false;
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
        if (UberManager.Instance.DevelopersMode)
        {
            gameOn = false;
            pause = true;

            UberManager.Instance.LevelEditor.GameOver();

            return;
        }

        pause = false;
        gameOn = false;

        if (LevelManager.Humans.Count > 0) won = true;

        UIManager.Instance.InGameUI.DeactivateBanners();

        if (UberManager.Instance.Tutorial)
        {
            UberManager.Instance.TutorialManager.Next();
            return;
        }

        // Switch game state
        UberManager.Instance.GotoState(UberManager.GameStates.PostGame);
    }

    public void RestartDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return;
        LevelManager.ResetTurns();
        UIManager.Instance.InGameUI.Pause(pause);
        tileManager.HidePossibleRoads();
        TileManager.FindNeighboursDEVMODE();
        gameOn = true;
    }

    // update is called every frame
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) Pause(!pause);

        if (pause)
        {
            cameraManager.UpdateDEVMODE();
            return;
        }

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

    public void Pause(bool on)
    {
        // currently paused, trying to unpause and level isn't playable
        // or currently unpaused, trying to pause and level isn't pausable
        if (!on && !UberManager.Instance.LevelEditor.CurrentLevelIsPlayable() ||
            on && !UberManager.Instance.LevelEditor.CurrentLevelIsPausable())
            return;

        pause = on;
        UberManager.Instance.LevelEditor.Pause(pause);
        UIManager.Instance.InGameUI.Pause(pause);

        // currently unpaused, now pausing
        if (pause)
        {
            // reset all the still active objects
            levelManager.ResetAllDEVMODE();

            // hide highlighted rules
            tileManager.HidePossibleRoads();
        }

        TileManager.FindNeighboursDEVMODE();
    }
}
