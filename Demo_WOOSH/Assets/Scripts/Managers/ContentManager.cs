using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public struct SpawnNode
{
    public TileManager.ContentType type;
    public Coordinate position;
}

[Serializable]
public class ContentManager {
    private static ContentManager instance = null;
    public static ContentManager Instance
    {
        get
        {
            if (instance == null) instance = UberManager.Instance.ContentManager;
            return instance;
        }
    }

    public GameObject Barrel { get; private set; }
    public GameObject Shrine { get; private set; }
    public GameObject Goo { get; private set; }

    public List<GameObject> Humans { get; private set; }
    public List<GameObject> Bosses { get; private set; }
    public List<GameObject> Minions { get; private set; }

    [SerializeField] private LevelDataContainer levelDataContainer = new LevelDataContainer();
    public LevelDataContainer LevelDataContainer { get { return levelDataContainer; } }

    public void Initialize()
    {
        Barrel = Resources.Load<GameObject>("Prefabs/Barrel");
        Shrine = Resources.Load<GameObject>("Prefabs/Shrine");
        Goo = Resources.Load<GameObject>("Prefabs/Hole");

        Humans = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Humans"));
        Bosses = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Creatures"));
        Minions = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Minions"));

        ReadLevelData();
    }

    private void ReadLevelData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LevelDataContainer));
        TextReader textReader = new StreamReader(Application.streamingAssetsPath + "/LevelData.xml");

        levelDataContainer = (LevelDataContainer)serializer.Deserialize(textReader);

        textReader.Close();
    }

    public void SaveAllInformation()
    {
        FileStream fs = new FileStream(Application.streamingAssetsPath + "/LevelData.xml", FileMode.Create);

        XmlSerializer serializer = new XmlSerializer(typeof(LevelDataContainer));

        serializer.Serialize(fs, levelDataContainer);

        fs.Close();
    }
}
