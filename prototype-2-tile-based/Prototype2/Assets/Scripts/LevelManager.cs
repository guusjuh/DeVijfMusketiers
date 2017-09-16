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
    public GameObject vase;
    public GameObject human;
    public GameObject creature;
    public GameObject hole;

    private List<Human> humans; 

    // used for init of level
    private Transform boardHolder;
    private List<Vector2> gridPositions = new List<Vector2>();

    // called by GM to set up the scene
    public void SetupScene()
    {
        // load and set up 
        LoadTiles();
        BoardSetup();
        InitialiseList();

        // remove creature position
        Vector2 centerPosition = new Vector2((int)(columns / 2.0f), (int)(rows / 2.0f));
        gridPositions.RemoveAt((int)((rows * centerPosition.y) + centerPosition.x));

        // instantiate creature
        // init done in GM (yea not so pritty, i know, i know)
        creature = (GameObject)GameObject.Instantiate(creature, centerPosition, Quaternion.identity);
        hole = (GameObject)GameObject.Instantiate(hole, centerPosition, Quaternion.identity);

        // init world objects
        LayoutObjectAtRandom(vase, 3, 6);
        LayoutObjectAtRandom(human, 2, 4);

        humans = new List<Human>();
        humans.AddMultiple(GameObject.FindObjectsOfType(typeof(Human)) as Human[]);
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
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
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
    }

    // instantiates object at random location
    void LayoutObjectAtRandom(GameObject tile, int min, int max)
    {
        int objectCount = UnityEngine.Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject.Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }

    // returns a random free position 
    Vector2 RandomPosition()
    {
        int randomIndex = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void RemoveHuman(Human toRemove)
    {
        humans.Remove(toRemove);
        GameObject.Destroy(toRemove.gameObject);

        if (humans.Count <= 0)
        {
            Application.LoadLevel("Lose");
        }
    }
}
