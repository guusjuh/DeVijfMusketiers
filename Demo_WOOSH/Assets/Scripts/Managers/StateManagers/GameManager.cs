using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameManager : StateManager {
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
            if (instance == null) instance = UberManager.Instance.GameManager;
            return instance;
        }
    }

    private bool gameOn = false;

    private LevelManager levelManager = new LevelManager();
    public LevelManager LevelManager { get { return levelManager; } }

    private TileManager tileManager = new TileManager();
    public TileManager TileManager { get { return tileManager; } }

    //TODO: enum for layers!!

    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    //TODO
    // 1st contenttype becomes content (THIS spefic boss)
    // 2nd contenttype stays the same
    private Dictionary<TileManager.ContentType, List<TileManager.ContentType>> typesToEnter = new Dictionary<TileManager.ContentType, List<TileManager.ContentType>>();
    public Dictionary<TileManager.ContentType, List<TileManager.ContentType>> TypesToEnter { get { return typesToEnter; } }

    public override void Initialize()
    { 
        SetTypesToEnter();

        tileManager.Initialize();
        UIManager.Instance.InGameUI.Initialize();
        levelManager.Initialize();

        gameOn = true;
    }

    public override void Restart()
    {
        tileManager.Restart();
        UIManager.Instance.InGameUI.Restart();
        levelManager.Restart();

        gameOn = true;
    }

    public override void Clear()
    {
        gameOn = false;

        // call all clear methods
        levelManager.Clear();
        UIManager.Instance.InGameUI.Clear();
        tileManager.ClearGrid();

        // Activate the garbage collector so we start clean.
        GC.Collect();

        // Unload all unused assets to clear memory.
        Resources.UnloadUnusedAssets();

        //RestartGame();
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
        levelManager.Update();
    }

    public void SkipPlayerTurn()
    {
        levelManager.EndPlayerMove(1, true);
        UIManager.Instance.InGameUI.HideSpellButtons();
    }
}
