using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int amountOfTurns = 0;
    private Text warningText;

    // voor player
    private int totalActionPoints = 3;       // total points
    private int currentActionPoints;        // points left this turn

    public GameObject FloorTile { get; private set; }

    [SerializeField] private LevelManager levelManager = new LevelManager();
    public LevelManager LevelManager { get { return levelManager; } }

    private Creature creature;
    public Creature Creature {get { return creature; } }

    private List<Minion> creatures;
    public List<Minion> Creatures { get { return creatures; } }

    private List<SpellButton> spellButtons;
    private SkipButton skipButton;

    public void AddCreature(Minion enemy)
    {
        creatures.Add(enemy);
    }

    public void RemoveCreature(Minion enemy)
    {
        creatures.Remove(enemy);

        // if everything dead
        if(creature == null && creatures.Count <= 0)
        {
            Application.LoadLevel("Win");
        }
    }

    public void BossDead()
    {
        creature = null;
        
        // if everything is dead
        if (creatures.Count <= 0)
        {
            Application.LoadLevel("Win");
        }
    }

    private IEnumerator WarningText()
    {
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        warningText.gameObject.SetActive(false);
    }

    public void BeginPlayerTurn()
    {
        amountOfTurns++;
        if (amountOfTurns == 4)
        {
            StartCoroutine(WarningText());
        }

        if (amountOfTurns > 5)
        {
            levelManager.SpawnGoo();
        }

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

            //UpdateEnemyPaths();

            if(creature != null) Creature.EndPlayerTurn();

            playersTurn = false;
        }
        else
        {
            ActivateButtons();
        }
    }

    /*public void UpdateEnemyPaths()
    {
        Instance.Creatures.HandleAction(c => c.UpdateTarget());
        if(creature != null)
            Creature.UpdateTarget();
    }*/

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
        warningText = GameObject.FindGameObjectWithTag("WarningText").GetComponent<Text>();
        warningText.gameObject.SetActive(false);

        // load floor from resources
        FloorTile = Resources.Load<GameObject>("Prefabs/FloorTile");

        // level generation
        levelManager.SetupScene();

        // camera set up
        Camera camera = FindObjectOfType<Camera>();
        camera.transform.position = new Vector3((levelManager.columns / 2.0f) - 0.5f, (levelManager.rows / 2.0f) - 0.5f, -10.0f);

        // find the creature
        creatures = new List<Minion>();
        //Minion[] tempTargets = FindObjectsOfType(typeof(Minion)) as Minion[];
        //creatures.AddMultiple(tempTargets);

        //creatures.HandleAction(c => c.Initialize();

        creature = FindObjectOfType<Creature>() as Creature;
        creature.Initialize((int)(levelManager.columns / 2.0f), (int)(levelManager.rows / 2.0f));

        // find all buttons
        spellButtons = new List<SpellButton>();
        spellButtons.AddMultiple(FindObjectsOfType<SpellButton>() as SpellButton[]);
        
        skipButton = FindObjectOfType<SkipButton>() as SkipButton;

        DeactivateButtons();

        // start with player turn
        playersTurn = true;
        BeginPlayerTurn();
        othersTurn = false;
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

        if (creatures.Count > 0)
        {
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
        }

        if (creature != null)
        {
            creature.StartTurn();
            while (creature.CurrentActionPoints > 0)
            {
                // make creature move
                creature.MoveEnemy();

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