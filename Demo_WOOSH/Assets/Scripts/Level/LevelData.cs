﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LevelData
{
    [SerializeField] public int id;
    [SerializeField] public int rows;
    [SerializeField] public int columns;
    [SerializeField] public SecTileTypeRow[] grid;
    [SerializeField] public List<SpawnNode> spawnNodes;
    public int amountOfHumans { get { return spawnNodes.FindAll(s => s.secType == SecContentType.Human).Count; } }
}

[Serializable]
public class SecTileTypeRow
{
    [SerializeField] public SecTileType[] row;
}

[Serializable]
public class LevelDataContainer
{
    [SerializeField] private List<LevelData> levelData = new List<LevelData>();
    public List<LevelData> LevelData { get { return levelData; } }
}
