using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEditorInternal;

[Serializable]
public struct SpawnNode
{
    public ContentType type;
    public SecContentType secType;
    public Coordinate position;
}

public enum TileType
{
    Unknown = -1,
    Normal = 0,
    Dangerous,
}

public enum SecTileType
{
    Unknown = -1,
    Dirt,
    Grass,
    Gap,
}

public enum ContentType
{
    Unknown = -1,
    Human = 0,
    Boss,
    Minion,
    Environment,
}

public enum SecContentType
{
    Unknown = -1,
    Human = 0,
    Dodin,
    Arnest,
    Sketta,
    Wolf,
    Barrel,
    Shrine
}

public enum HumanTypes
{
    Bad,
    Ok,
    Normal,
    Good,
    Fabulous
}

[Serializable]
public class ContentManager {
    private static ContentManager instance = null;
    public static ContentManager Instance {
        get {
            if (instance == null) instance = UberManager.Instance.ContentManager;
            return instance;
        }
    }

    private static Dictionary<ContentType, List<SecContentType>> validContentTypes;
    public static Dictionary<ContentType, List<SecContentType>> ValidContentTypes { get {return validContentTypes; } }
    private static Dictionary<TileType, List<SecTileType>> validTileTypes;
    public static Dictionary<TileType, List<SecTileType>> ValidTileTypes { get { return validTileTypes; } }

    private Dictionary<KeyValuePair<ContentType, SecContentType>, GameObject> contentPrefabs = new Dictionary<KeyValuePair<ContentType, SecContentType>, GameObject>();
    public Dictionary<KeyValuePair<ContentType, SecContentType>, GameObject> ContentPrefabs { get { return contentPrefabs; } }

    private Dictionary<KeyValuePair<TileType, SecTileType>, GameObject> tilePrefabs = new Dictionary<KeyValuePair<TileType, SecTileType>, GameObject>();
    public Dictionary<KeyValuePair<TileType, SecTileType>, GameObject> TilePrefabs { get { return tilePrefabs; } }

    //TODO: same as content but for tiles
    public GameObject Gap { get; private set; }

    [SerializeField] private LevelDataContainer levelDataContainer = new LevelDataContainer();

    public LevelData LevelData(int id)
    {
        if (id > levelDataContainer.LevelData.Count) return null;
        return levelDataContainer.LevelData[id];
    }
    public int AmountOfLevels { get { return levelDataContainer.LevelData.Count; } }

    public void Initialize() {
        Gap = Resources.Load<GameObject>("Prefabs/Hole");

        SetValidTypes();

        LoadPrefabsForContentType("Prefabs/Bosses", ContentType.Boss);
        LoadPrefabsForContentType("Prefabs/Minions", ContentType.Minion);
        LoadPrefabsForContentType("Prefabs/Humans", ContentType.Human);
        LoadPrefabsForContentType("Prefabs/Environment", ContentType.Environment);

        LoadPrefabsForTileType("Prefabs/Tiles/Normal", TileType.Normal);
        LoadPrefabsForTileType("Prefabs/Tiles/Dangerous", TileType.Dangerous);

        //ReadLevelData();
    }

    private void LoadPrefabsForContentType(String toLoadString, ContentType type)
    {
        List<GameObject> tempPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(toLoadString));
        KeyValuePair<ContentType, SecContentType> tempKeyValuePair = new KeyValuePair<ContentType, SecContentType>();

        for (int i = 0; i < tempPrefabs.Count; i++)
        {
            for (int j = 0; j < validContentTypes[type].Count; j++)
            {
                String name = tempPrefabs[i].name;
                String enumName = validContentTypes[type][j].ToString();

                if (name == enumName)
                {
                    tempKeyValuePair = new KeyValuePair<ContentType, SecContentType>(type, validContentTypes[type][j]);
                    contentPrefabs.Add(tempKeyValuePair, tempPrefabs[i]);
                }
            }
        }
    }

    private void LoadPrefabsForTileType(String toLoadString, TileType type)
    {
        List<GameObject> tempPrefabs = new List<GameObject>(Resources.LoadAll<GameObject>(toLoadString));
        KeyValuePair<TileType, SecTileType> tempKeyValuePair = new KeyValuePair<TileType, SecTileType>();

        for (int i = 0; i < tempPrefabs.Count; i++)
        {
            for (int j = 0; j < validTileTypes[type].Count; j++)
            {
                String name = tempPrefabs[i].name;
                String enumName = validTileTypes[type][j].ToString();

                if (name == enumName)
                {
                    tempKeyValuePair = new KeyValuePair<TileType, SecTileType>(type, validTileTypes[type][j]);
                    tilePrefabs.Add(tempKeyValuePair, tempPrefabs[i]);
                }
            }
        }
    }

    private void SetValidTypes()
    {
        validContentTypes = new Dictionary<ContentType, List<SecContentType>>();

        List<SecContentType> humans = new List<SecContentType>();
        humans.Add(SecContentType.Human);
        List<SecContentType> bosses = new List<SecContentType>();
        bosses.Add(SecContentType.Dodin);
        bosses.Add(SecContentType.Arnest);
        bosses.Add(SecContentType.Sketta);
        List<SecContentType> minions = new List<SecContentType>();
        minions.Add(SecContentType.Wolf);
        List<SecContentType> environmentals = new List<SecContentType>();
        environmentals.Add(SecContentType.Barrel);
        environmentals.Add(SecContentType.Shrine);

        validContentTypes.Add(ContentType.Human, humans);
        validContentTypes.Add(ContentType.Boss, bosses);
        validContentTypes.Add(ContentType.Minion, minions);
        validContentTypes.Add(ContentType.Environment, environmentals);

        validTileTypes = new Dictionary<TileType, List<SecTileType>>();

        List<SecTileType> normals = new List<SecTileType>();
        normals.Add(SecTileType.Dirt);
        normals.Add(SecTileType.Grass);
        List<SecTileType> dangerZones = new List<SecTileType>();
        dangerZones.Add(SecTileType.Gap);

        validTileTypes.Add(TileType.Normal, normals);
        validTileTypes.Add(TileType.Dangerous, dangerZones);
    }

    static public bool IsValidSecContentType(ContentType contentType, SecContentType secContentType)
    {
        return validContentTypes.ContainsKey(contentType) && validContentTypes[contentType].Contains(secContentType);
    }

    static public ContentType GetPrimaryFromSecContent(SecContentType secContentType)
    {
        ContentType type = validContentTypes.FirstOrDefault(v => v.Value.Contains(secContentType)).Key;

        if(!IsValidSecContentType(type, secContentType)) return ContentType.Unknown;

        return type;
    }

    static public bool IsValidSecTileType(TileType contentType, SecTileType secContentType)
    {
        return validTileTypes.ContainsKey(contentType) && validTileTypes[contentType].Contains(secContentType);
    }

    static public TileType GetPrimaryFromSecTile(SecTileType secContentType)
    {
        TileType type = validTileTypes.FirstOrDefault(v => v.Value.Contains(secContentType)).Key;

        if (!IsValidSecTileType(type, secContentType)) return TileType.Unknown;

        return type;
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
