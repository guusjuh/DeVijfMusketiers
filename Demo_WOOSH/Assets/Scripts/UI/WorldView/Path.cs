using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
    private List<LevelSelectParent> levels;
    public List<LevelSelectParent> Levels { get { return levels; } }

    private Destination destination;
    public Destination Destination {get {return destination;} }

    private int pathId;
    private City city;
    private City MyCity { get { return city; } }

    public Path(Transform pathObject, City city, int pathId, Destination destination)
    {
        this.city = city;
        this.destination = destination;
        this.pathId = pathId;
        levels = new List<LevelSelectParent>(pathObject.GetComponentsInChildren<LevelSelectParent>());
        
        int levelCount = 0;
        levels.HandleAction(l =>
        {
            l.Initialize(this, levelCount);
            levelCount++;
        });
    }

    public bool SpawnContract(Contract contract)
    {
        //spawning a contract, so always level 0
        bool spaceInThisLevel = UberManager.Instance.ContractManager.AmountOfContracts(levels[0].LevelID) < 6;

        if (spaceInThisLevel)
        {
            UberManager.Instance.ContractManager.AddContract(contract);
            levels[0].AddHuman();
            levels.HandleAction(l => l.CheckActiveForButton());

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
