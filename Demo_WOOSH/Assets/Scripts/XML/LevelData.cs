using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    [SerializeField] public int id;
    [SerializeField] public int rows;
    [SerializeField] public int columns;
    [SerializeField] public List<SpawnNode> spawnNodes;
    [SerializeField] public Coordinate gooStartPos;
    [SerializeField] public int minAmountOfHumans;
    [SerializeField] public int maxAmountOfHumans;
    [SerializeField] public int bossID;
}

[Serializable]
public class LevelDataContainer
{
    [SerializeField] private List<LevelData> levelData = new List<LevelData>();
    public List<LevelData> LevelData { get { return levelData; } }
}
