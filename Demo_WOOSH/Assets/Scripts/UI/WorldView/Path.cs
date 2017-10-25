using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path {
    private List<LevelSelectParent> levels;
    public List<LevelSelectParent> Levels { get { return levels; } }

    private int pathId;
    private City city;

    public Path(Transform pathObject, City city, int pathId)
    {
        this.city = city;
        this.pathId = pathId;
        levels = new List<LevelSelectParent>(pathObject.GetComponentsInChildren<LevelSelectParent>());
        
        int levelCount = 0;
        levels.HandleAction(l =>
        {
            l.Initialize(this, levelCount);
            levelCount++;
        });
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
