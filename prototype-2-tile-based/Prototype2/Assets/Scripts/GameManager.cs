using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamagableType
{
    Human = 0,
    Vase
}

public class GameManager : MonoBehaviour {
    // singleton
    private static GameManager instance = null;

    public static GameManager Instance {
        get {
            if (instance == null) instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return instance;
        }
    }

    public bool playersTurn = false;         // bool to check if it's players turn, hidden in inspector but public
    protected bool othersTurn = false;
    public float turnDelay = 0.1f;          // delay between each players turn
    private bool doingSetup = true;

    public GameObject FloorTile { get; private set; }

    private LevelManager levelManager;
    public LevelManager LevelManager { get { return levelManager; } }
    private Creature creature;
    public Creature Creature { get { return creature; } }

    public void Awake()
    {
        // block all input while setting up the game
        doingSetup = true;

        // init game and world
        InitGame();
    }

    private void InitGame()
    {
        // load floor from resources
        FloorTile = Resources.Load<GameObject>("Prefabs/FloorTile");

        // level generation
        levelManager = new LevelManager();
        levelManager.SetupScene();

        // camera set up
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3(levelManager.columns / 2.0f, (levelManager.rows / 2.0f) - 0.5f, -10.0f);

        // find the creature
        creature = FindObjectOfType<Creature>();
        creature.Initialize();
    }

    // update is called every frame
    public void Update()
    {
        if (playersTurn || othersTurn)
            return;

        // start moving enemies
        StartCoroutine(HandleOtherTurn());
    }

    // called when the player looses all humans
    public void GameOver()
    {
        //DIEE
    }

    // move creature
    protected IEnumerator HandleOtherTurn()
    {
        //Debug.Log("Handling creature turn");

        othersTurn = true;

        // wait for turn delay
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < creature.totalActionPoints; i++)
        {
            // make creature move
            creature.MoveEnemy();

            // delay
            yield return new WaitForSeconds(1.5f);
        }

        // switch turns
        playersTurn = true;
        othersTurn = false;
    }
}
