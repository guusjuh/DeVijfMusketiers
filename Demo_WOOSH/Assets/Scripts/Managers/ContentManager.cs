using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentManager {
    public GameObject Barrel { get; private set; }
    public GameObject Shrine { get; private set; }
    public GameObject Goo { get; private set; }

    public List<GameObject> Humans { get; private set; }
    public List<GameObject> Bosses { get; private set; }
    public List<GameObject> Minions { get; private set; }

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
