using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all instances in the game 
/// </summary>
public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    [SerializeField]
    private GameObject spawnObject;
    [SerializeField]
    private GameObject endObject;

    public PlayerScript Player { get; private set; }
    public UfoScript UFO { get; private set; }
    public List<LiftableObject> Crates { get; private set; }

    public bool InGame { get; private set; }

    public void Start()
    {
        InGame = false;

        Player = FindObjectOfType<PlayerScript>();
        Player.Initialize(spawnObject.transform.position);

        UFO = FindObjectOfType<UfoScript>();
        UFO.Initialize();

        Crates = new List<LiftableObject>();
        LiftableObject[] temp = FindObjectsOfType<LiftableObject>();
        foreach(LiftableObject o in temp)
        {
            Crates.Add(o);
        }

        for (int i = 0; i < Crates.Count; ++i)
        {
            Crates[i].Initialize();
        }

        return;
    }

    public void Update()
    {
        Player.Loop();
        UFO.Loop();

        for (int i = 0; i < Crates.Count; ++i)
        {
            Crates[i].Loop();
        }
        return;
    }
}
