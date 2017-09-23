using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamagableType
{
    Human = 0,
    Barrel,
    Shrine
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

    // voor player
    private int totalActionPoints = 3;       // total points
    private int currentActionPoints;        // points left this turn

    public GameObject FloorTile { get; private set; }

    [SerializeField] private LevelManager levelManager = new LevelManager();

    public LevelManager LevelManager
    {
        get { return levelManager; }
    }

    private List<Creature> creatures;

    public List<Creature> Creatures
    {
        get { return creatures; }
    }

    private List<SpellButton> spellButtons;
    private SkipButton skipButton;

    public void BeginPlayerTurn()
    {
        int addedActionPoints = 0;
        for (int i = 0; i < levelManager.Shrines.Count; i++)
        {
            addedActionPoints += levelManager.Shrines[i].GetActionPoints();
        }
        currentActionPoints = totalActionPoints + addedActionPoints;

        Debug.Log(currentActionPoints);

        ActivateButtons();
        Human[] humans = FindObjectsOfType<Human>() as Human[];
        for (int i = 0; i < humans.Length; i++)
        {
            humans[i].BeginPlayerTurn();
        }

        spellButtons.HandleAction(b => b.EndPlayerTurn());
    }

    public void EndPlayerTurn(int cost = 1, bool endTotally = false)
    {
        currentActionPoints = endTotally ? 0 : currentActionPoints - cost;

        if (currentActionPoints <= 0)
        {
            DeactivateButtons();

            playersTurn = false;
        }
        else
        {
            ActivateButtons();
        }
    }

    public void SkipButtonClick()
    {
        EndPlayerTurn(1, true);
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
        creatures = new List<Creature>();
        Creature[] tempTargets = FindObjectsOfType(typeof(Creature)) as Creature[];
        creatures.AddMultiple(tempTargets);

        creatures.HandleAction(c => c.Initialize((int) (levelManager.columns / 2.0f), (int) (levelManager.rows / 2.0f)));

        // find all buttons
        spellButtons = new List<SpellButton>();
        spellButtons.AddMultiple(FindObjectsOfType<SpellButton>() as SpellButton[]);
        
        skipButton = FindObjectOfType<SkipButton>() as SkipButton;

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

        creatures.HandleAction(c => c.StartTurn());

        for (int i = 0; i < creatures.Count; i++)
        {
            while (creatures[i].CurrentActionPoints > 0)
            {
                // make creature move
                creatures[i].MoveEnemy();

                // delay
                yield return new WaitForSeconds(0.6f);
            }
        }

        // switch turns
        playersTurn = true;
        BeginPlayerTurn();
        othersTurn = false;
    }

    private void ActivateButtons()
    {
        spellButtons.HandleAction(b => b.Active = true);
        skipButton.Active = true;
    }

    public void DeactivateButtons()
    {
        spellButtons.HandleAction(b => b.Active = false);
        skipButton.Active = false;
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