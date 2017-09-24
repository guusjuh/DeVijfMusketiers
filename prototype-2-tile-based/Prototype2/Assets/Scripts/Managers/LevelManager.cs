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
    private GameObject barrel;
    private GameObject human;
    private GameObject shrine;
    private GameObject creature;
    private GameObject minion;
    private GameObject smug;

    private List<Human> humans;
    private List<Shrine> shrines;
    public  List<Shrine> Shrines { get { return shrines; } }

    // used for init of level
    private Transform boardHolder;
    private List<Vector2> gridPositions = new List<Vector2>();

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

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
       // minion = (GameObject)GameObject.Instantiate(minion, centerPosition, Quaternion.identity);
        smug = (GameObject)GameObject.Instantiate(smug, centerPosition, Quaternion.identity);

        // init world objects
        List<Vector2> humanSpawnPosses = new List<Vector2>();
        humanSpawnPosses.Add(new Vector2(0,0));
        humanSpawnPosses.Add(new Vector2(4,3));
        humanSpawnPosses.Add(new Vector2(4,6));

        for (int i = 0; i < humanSpawnPosses.Count; i++)
        {
            GameObject go = GameObject.Instantiate(human, humanSpawnPosses[i], Quaternion.identity);
            generatedLevel[(int)humanSpawnPosses[i].x, (int)humanSpawnPosses[i].y] = TileMap.Types.Human;
            gridPositions.Remove(humanSpawnPosses[i]);
            go.GetComponent<Human>().Initialize((int)humanSpawnPosses[i].x, (int)humanSpawnPosses[i].y);
        }

        List<Vector2> shrineSpawnPosses = new List<Vector2>();
        shrineSpawnPosses.Add(new Vector2(1, 2));
        shrineSpawnPosses.Add(new Vector2(3, 1));
        
        for (int i = 0; i < shrineSpawnPosses.Count; i++)
        {
            GameObject go = GameObject.Instantiate(shrine, shrineSpawnPosses[i], Quaternion.identity);
            generatedLevel[(int)shrineSpawnPosses[i].x, (int)shrineSpawnPosses[i].y] = TileMap.Types.Shrine;
            gridPositions.Remove(shrineSpawnPosses[i]);
            go.GetComponent<Shrine>().Initialize((int)shrineSpawnPosses[i].x, (int)shrineSpawnPosses[i].y);
        }

        List<Vector2> barrelSpawnPosses = new List<Vector2>();
        barrelSpawnPosses.Add(new Vector2(1, 3));
        barrelSpawnPosses.Add(new Vector2(1, 4));
        barrelSpawnPosses.Add(new Vector2(1, 5));
        barrelSpawnPosses.Add(new Vector2(3, 4));
        barrelSpawnPosses.Add(new Vector2(3, 6));
        barrelSpawnPosses.Add(new Vector2(2, 2));
        barrelSpawnPosses.Add(new Vector2(4, 4));

        for (int i = 0; i < barrelSpawnPosses.Count; i++)
        {
            GameObject go = GameObject.Instantiate(barrel, barrelSpawnPosses[i], Quaternion.identity);
            generatedLevel[(int)barrelSpawnPosses[i].x, (int)barrelSpawnPosses[i].y] = TileMap.Types.Barrel;
            gridPositions.Remove(barrelSpawnPosses[i]);
            go.GetComponent<Barrel>().Initialize((int)barrelSpawnPosses[i].x, (int)barrelSpawnPosses[i].y);
        }

        humans = new List<Human>();
        humans.AddMultiple(GameObject.FindObjectsOfType(typeof(Human)) as Human[]);

        shrines = new List<Shrine>();
        shrines.AddMultiple(GameObject.FindObjectsOfType(typeof(Shrine)) as Shrine[]);

        // set up the tilemap for A*
        tileMap.Initialize(columns, rows, generatedLevel);
        
        //initialize first spot of goo
        tileMap.SetObject((int)centerPosition.x, (int)centerPosition.y, TileMap.Types.Goo);
    }

    // loads all objects/tiles required to build the level
    void LoadTiles()
    {
        barrel = Resources.Load<GameObject>("Prefabs/Barrel");
        human = Resources.Load<GameObject>("Prefabs/Human");
        shrine = Resources.Load<GameObject>("Prefabs/Shrine");
        creature = Resources.Load<GameObject>("Prefabs/Creature");
        minion = Resources.Load<GameObject>("Prefabs/Minion");
        smug = Resources.Load<GameObject>("Prefabs/Hole");
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

                if (x < 0 || x >= columns || y < 0 || y >= rows)
                    instance.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1);
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

        if (humans.Count <= 0)
        {
            Application.LoadLevel("Lose");
        }

        List<Shrine> all = new List<Shrine>();
        all.AddMultiple(GameObject.FindObjectsOfType<Shrine>() as Shrine[]);

        all.HandleAction(s => s.CheckForActive());
    }

    public void RemoveShrine(Shrine toRemove)
    {
        shrines.Remove(toRemove);
        tileMap.RemoveObject(toRemove.x, toRemove.y);
        GameObject.Destroy(toRemove.gameObject);
    }

    public void SpawnGoo()
    {
        // wat is allemaal al goo
        TileMap.Types[,] map = TileMap.Tiles;
        List<Vector2> gooPostions = new List<Vector2>();
        List<Vector2> posGooPostions = new List<Vector2>();
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (map[x, y] == TileMap.Types.Goo)
                {
                    gooPostions.Add(new Vector2(x, y));
                }
            }
        }
        
        // waar kunnen we dan spawnen
        for (int i = 0; i < gooPostions.Count; i++)
        {
            for (int j = 0; j < positions.Length; j++)
            {
                Vector2 curPos = gooPostions[i] + (Vector2)positions[j];
                if (gooPostions.Contains(curPos) ||
                    ((int)curPos.x <= 0 || (int)curPos.x > rows - 1) ||
                    ((int)curPos.y <= 0 || (int)curPos.y > columns - 1))
                    continue;
                else
                {
                    posGooPostions.Add(curPos);
                }
                    
            }
        }
        // select een tegel
        int rnd = UnityEngine.Random.Range(0, posGooPostions.Count);
        Vector2 theChosenGoo = posGooPostions[rnd];
        Debug.Log("Chose goo {" + theChosenGoo.x + "," + theChosenGoo + "}");

        // kill all stuff on de tegel
        TileMap.Types t = TileMap.Tiles[(int) theChosenGoo.x, (int) theChosenGoo.y];
        switch (t)
        {
            case TileMap.Types.Human:
            case TileMap.Types.InvisibleHooman:
                List<Human> humans = new List<Human>();
                humans.AddMultiple(GameObject.FindObjectsOfType<Human>() as Human[]);
                for (int i = 0; i < humans.Count; i++)
                {
                    if (Mathf.Abs(humans[i].x - theChosenGoo.x) <= 0.1f &&
                        Mathf.Abs(humans[i].y - theChosenGoo.y) <= 0.1f)
                    {
                        humans[i].Hit();
                    }
                }
                break;
            case TileMap.Types.Barrel:
            case TileMap.Types.BrokenBarrel:
                List<Barrel> barrels = new List<Barrel>();
                barrels.AddMultiple(GameObject.FindObjectsOfType<Barrel>() as Barrel[]);
                for (int i = 0; i < barrels.Count; i++)
                {
                    if (Mathf.Abs(barrels[i].x - theChosenGoo.x) <= 0.1f &&
                        Mathf.Abs(barrels[i].y - theChosenGoo.y) <= 0.1f)
                    {
                        barrels[i].Remove();
                    }
                }
                break;
            case TileMap.Types.Shrine:
                List<Shrine> shrines = new List<Shrine>();
                shrines.AddMultiple(GameObject.FindObjectsOfType<Shrine>() as Shrine[]);
                for (int i = 0; i < shrines.Count; i++)
                {
                    if (Mathf.Abs(shrines[i].x - theChosenGoo.x) <= 0.1f &&
                        Mathf.Abs(shrines[i].y - theChosenGoo.y) <= 0.1f)
                    {
                        shrines[i].Hit();
                    }
                }
                break;
        }

        // add in tilemanager
        smug = (GameObject)GameObject.Instantiate(smug, theChosenGoo, Quaternion.identity);
        tileMap.SetObject((int)theChosenGoo.x, (int)theChosenGoo.y, TileMap.Types.Goo);
    }
}
