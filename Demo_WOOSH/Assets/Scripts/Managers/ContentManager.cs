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

    private List<Sprite> WorldHumans { get; set; }
    private List<Sprite> PortraitHumans { get; set; }
    private Dictionary<HumanTypes, List<Sprite>> HumanSprites { get; set; }

    //TODO: at later moment, maybe different human prefabs needed for the different behaviors!
    public GameObject Human { get; private set; }

    public List<GameObject> Bosses { get; private set; }
    public List<GameObject> Minions { get; private set; }

    [SerializeField] private LevelDataContainer levelDataContainer = new LevelDataContainer();
    public LevelDataContainer LevelDataContainer { get { return levelDataContainer; } }

    public void Initialize()
    {
        Barrel = Resources.Load<GameObject>("Prefabs/Barrel");
        Shrine = Resources.Load<GameObject>("Prefabs/Shrine");
        Goo = Resources.Load<GameObject>("Prefabs/Hole");

        WorldHumans = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/Humans"));
        PortraitHumans = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/UI/HumanIcons"));
        HumanSprites = new Dictionary<HumanTypes, List<Sprite>>();
        for (int i = 0; i < WorldHumans.Count; i++)
        {
            HumanSprites.Add((HumanTypes)i, new List<Sprite>());
            HumanSprites.Get((HumanTypes)i).Add(WorldHumans[i]);
            HumanSprites.Get((HumanTypes)i).Add(PortraitHumans[i]);
        }

        Human = Resources.Load<GameObject>("Prefabs/Humans/Human");

        Bosses = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Creatures"));
        Minions = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Minions"));

        ReadLevelData();
    }

    public enum HumanTypes
    {
        Normal,
        Ok, 
        Good
    }

    public List<Sprite> GetHumanSprites(HumanTypes type)
    {
        return HumanSprites.Get(type);
    }

    private void ReadLevelData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(LevelDataContainer));
        string path = "LevelData";
        TextAsset file = Resources.Load(path) as TextAsset;
        TextReader textReader = new System.IO.StringReader(file.text);

        levelDataContainer = (LevelDataContainer)serializer.Deserialize(textReader);

        textReader.Close();
    }

    public void SaveAllInformation()
    {
        FileStream fs = new FileStream("Assets/Resources/LevelData.xml", FileMode.OpenOrCreate);

        XmlSerializer serializer = new XmlSerializer(typeof(LevelDataContainer));

        serializer.Serialize(fs, levelDataContainer);

        fs.Close();
    }
}
