using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpawnNode
{
    public TileManager.ContentType type;
    public Coordinate position;
}

[Serializable]
public struct LevelData
{
    public int rows;
    public int columns;
    [SerializeField] public List<SpawnNode> spawnNodes;
    public Coordinate gooStartPos;
    public int amountOfHumans;
}

[Serializable]
public class ContentManager {
    private static ContentManager instance = null;
    public static ContentManager Instance
    {
        get
        {
            if (instance == null) instance = GameManager.Instance.ContentManager;
            return instance;
        }
    }

    public GameObject Barrel { get; private set; }
    public GameObject Shrine { get; private set; }
    public GameObject Goo { get; private set; }

    public List<GameObject> Humans { get; private set; }
    public List<GameObject> Bosses { get; private set; }
    public List<GameObject> Minions { get; private set; }

    [SerializeField] private List<LevelData> levels;
    public List<LevelData> Levels { get { return levels; } }

    //TODO: level objects

    //TODO: ui objects 

    public void Initialize()
    {
        Barrel = Resources.Load<GameObject>("Prefabs/Barrel");
        Shrine = Resources.Load<GameObject>("Prefabs/Shrine");
        Goo = Resources.Load<GameObject>("Prefabs/Hole");

        Humans = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Humans"));
        Bosses = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Creatures"));
        Minions = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Minions"));
    }
}
