using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamagableType
{
    Human = 0,
    Vase
}

public class GameManager : MonoBehaviour
{
    // singleton
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            return instance;
        }
    }

    public bool playersTurn = false; // bool to check if it's players turn, hidden in inspector but public
    protected bool othersTurn = false;
    public float turnDelay = 0.1f; // delay between each players turn
    private bool doingSetup = true;

    public GameObject FloorTile { get; private set; }

    [SerializeField] private LevelManager levelManager = new LevelManager();

    public LevelManager LevelManager
    {
        get { return levelManager; }
    }

    private Creature creature;

    public Creature Creature
    {
        get { return creature; }
    }

    private List<SpellButton> spellButtons;

    public void BeginPlayerTurn()
    {
        ActivateButtons();
        Human[] humans = FindObjectsOfType<Human>() as Human[];
        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].BeginPlayerTurn();
        }
    }

    public void EndPlayerTurn()
    {
        DeactivateButtons();

        spellButtons.HandleAction(b => b.EndPlayerTurn());

        playersTurn = false;
    }

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
        levelManager.SetupScene();

        // camera set up
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3((levelManager.columns / 2.0f) - 0.5f, (levelManager.rows / 2.0f) - 0.5f, -10.0f);

        // find the creature
        creature = FindObjectOfType<Creature>();
        creature.Initialize((int) (levelManager.columns / 2.0f), (int) (levelManager.rows / 2.0f));

        // find all buttons
        spellButtons = new List<SpellButton>();
        spellButtons.AddMultiple(FindObjectsOfType<SpellButton>() as SpellButton[]);
        DeactivateButtons();

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

        creature.StartTurn();
        
        while(creature.CurrentActionPoints > 0)
        {
            // make creature move
            creature.MoveEnemy();

            // delay
            yield return new WaitForSeconds(1.0f);
        }

        // switch turns
        playersTurn = true;
        BeginPlayerTurn();
        othersTurn = false;
    }

    private void ActivateButtons()
    {
        spellButtons.HandleAction(b => b.Active = true);
    }

    public void DeactivateButtons()
    {
        spellButtons.HandleAction(b => b.Active = false);
    }

    public Vector2 WorldToCanvas(Vector3 worldPosition)
    {
        Camera camera = Camera.main;
        Canvas canvas = FindObjectOfType<Canvas>() as Canvas;

        var viewportPos = camera.WorldToViewportPoint(worldPosition);
        var canvasRect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
    }
}