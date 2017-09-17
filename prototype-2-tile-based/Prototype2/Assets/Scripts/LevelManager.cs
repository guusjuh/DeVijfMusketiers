using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelManager  {
    // size of level
    public int columns = 5;
    public int rows = 9;

    // world objects
    private GameObject vase;
    private GameObject human;
    private GameObject creature;
    private GameObject hole;

    private List<Human> humans; 

    // used for init of level
    private Transform boardHolder;
    private List<Vector2> gridPositions = new List<Vector2>();

    private TileMap.Types[,] generatedLevel;
    [SerializeField]
    private TileMap tileMap = new TileMap();
    public TileMap TileMap { get { return tileMap; } }

    // called by GM to set up the scene
    public void SetupScene()
    {
        // load and set up 
        LoadTiles();
        BoardSetup();
        InitialiseList();

        Vector2 centerPosition = new Vector2((int)(columns / 2.0f), (int)(rows / 2.0f));
        Vector2 up    = centerPosition + new Vector2(+1, 0);
        Vector2 down  = centerPosition + new Vector2(-1, 0);
        Vector2 left  = centerPosition + new Vector2(0, -1);
        Vector2 right = centerPosition + new Vector2(0, +1);

        // before removing, set types
        //generatedLevel[(int)centerPosition.x, (int)centerPosition.y] = TileMap.Types.Saved;
        //generatedLevel[(int)up.x, (int)up.y] = TileMap.Types.Saved;
        //generatedLevel[(int)down.x, (int)down.y] = TileMap.Types.Saved;
        //generatedLevel[(int)left.x, (int)left.y] = TileMap.Types.Saved;
        //generatedLevel[(int)right.x, (int)right.y] = TileMap.Types.Saved;

        // remove creature position
        // also remove up, down, left, right, so creature can always walk
        gridPositions.Remove(centerPosition);
        gridPositions.Remove(up);
        gridPositions.Remove(down);
        gridPositions.Remove(left);
        gridPositions.Remove(right);
        
        // instantiate creature
        // init done in GM (yea not so pritty, i know, i know)
        creature = (GameObject)GameObject.Instantiate(creature, centerPosition, Quaternion.identity);
        hole = (GameObject)GameObject.Instantiate(hole, centerPosition, Quaternion.identity);

        // init world objects
        LayoutObjectAtRandom(TileMap.Types.Vase, vase, 9, 12);//3, 6);
        LayoutObjectAtRandom(TileMap.Types.Human, human, 1, 3);//5, 7);

        humans = new List<Human>();
        humans.AddMultiple(GameObject.FindObjectsOfType(typeof(Human)) as Human[]);

        // set up the tilemap for A*
        tileMap.Initialize(columns, rows, generatedLevel);
    }

    // loads all objects/tiles required to build the level
    void LoadTiles()
    {
        vase = Resources.Load<GameObject>("Prefabs/Vase");
        human = Resources.Load<GameObject>("Prefabs/Human");
        creature = Resources.Load<GameObject>("Prefabs/Creature");
        hole = Resources.Load<GameObject>("Prefabs/Hole");
    }

    // sets up the level 
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform; //parent for tiles

        // loop through level
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -2; y < rows + 2; y++)
            {
                // instantiate tile
                GameObject instance = GameObject.Instantiate(GameManager.Instance.FloorTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                // set parent for pritty scene structure
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // inits all grid positions
    void InitialiseList()
    {
        gridPositions.Clear();

        // creating a list of possible positions
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector2(x, y));
            }
        }

        generatedLevel = new TileMap.Types[columns,rows];

        // creating a list of possible positions
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // instantiate everything at empty
                generatedLevel[x,y] = TileMap.Types.Empty;
            }
        }
    }

    // instantiates object at random location
    void LayoutObjectAtRandom(TileMap.Types replacementType, GameObject tile, int min, int max)
    {
        int objectCount = UnityEngine.Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition(replacementType);
            GameObject go = GameObject.Instantiate(tile, randomPosition, Quaternion.identity);
            go.GetComponent<Damagable>().Initialize((int)randomPosition.x, (int)randomPosition.y);
        }
    }

    // returns a random free position 
    Vector2 RandomPosition(TileMap.Types replacementType)
    {
        int randomIndex = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        generatedLevel[(int)randomPosition.x, (int)randomPosition.y] = replacementType;
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void RemoveHuman(Human toRemove)
    {
        humans.Remove(toRemove);
        tileMap.RemoveObject(toRemove.x, toRemove.y);
        GameObject.Destroy(toRemove.gameObject);

        //TODO: update tilemap

        if (humans.Count <= 0)
        {
            Application.LoadLevel("Lose");
        }
    }
}
