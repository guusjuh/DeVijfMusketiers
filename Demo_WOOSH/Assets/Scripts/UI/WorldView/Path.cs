﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
    private List<LevelSelectParent> levels;
    public List<LevelSelectParent> Levels { get { return levels; } }

    private Destination destination;
    public Destination Destination {get {return destination;} }

    private City city;
    private City MyCity { get { return city; } }

    private Transform pathObject;
    public Transform PathObject { get { return pathObject; } }

    public Path(Transform pathObject, City city, Destination destination)
    {
        this.pathObject = pathObject;
        this.city = city;
        this.destination = destination;
        levels = new List<LevelSelectParent>(pathObject.GetComponentsInChildren<LevelSelectParent>());
        
        int levelCount = 0;
        levels.HandleAction(l =>
        {
            l.Initialize(this, levelCount);
            levelCount++;
        });
    }

    public bool SpawnContract(Contract contract, int levelid = 0)
    {
        //spawning a contract, so always level 0
        bool spaceInThisLevel = UberManager.Instance.ContractManager.AmountOfContracts(levels[levelid].LevelID) < GameManager.AMOUNT_HUMANS_PER_LEVEL;

        if (spaceInThisLevel)
        {
            UberManager.Instance.ContractManager.AddContract(contract);
            levels[levelid].AddHuman();

            return true;
        }
        else
        {
            Debug.LogError("No space in this level");
            return false;
        }
    }

    public bool hasNextLevel(int levelInPathID)
    {
        return (levelInPathID + 1 < levels.Count);
    }

    public int GetNextLevelID(int levelInPathID)
    {
        if (hasNextLevel(levelInPathID)) return levels[levelInPathID + 1].LevelID;
        else return -1;
    }

    public void Clear()
    {
        levels.HandleAction(l => l.Clear());
    }

    public void Restart()
    {
        levels.HandleAction(l => l.Restart());
    }
}
