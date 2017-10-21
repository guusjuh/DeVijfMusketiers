using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private bool init = false;
    public bool Initialized { get { return init; } }
    int humansInstantiated = 0;

    private List<Human> humans;
    public List<Human> Humans { get { return humans; } }

    private List<Barrel> barrels; 
    public List<Barrel> Barrels { get { return barrels; } }

    private List<Shrine> shrines; 
    public List<Shrine> Shrines { get { return shrines; } }

    private List<Enemy> enemies;
    public List<Enemy> Enemies { get { return enemies; } }

    private Player player;
    public Player Player { get { return player; } }

    private bool playersTurn = false;
    private bool othersTurn = false;
    public bool PlayersTurn { get { return playersTurn; } }
    private int extraPoints;

    private int amountOfTurns = 0;
    public int AmountOfTurns { get { return amountOfTurns; } }

    private float delay = 0.5f;
    private int startGapTurn = 2;

    public void Initialize()
    {
        SetUpLevel();
        init = true;
    }

    public void Restart()
    {
        SetUpLevel();
    }

    private void SetUpLevel()
    {
        humansInstantiated = 0;

        humans = new List<Human>();
        barrels = new List<Barrel>();
        shrines = new List<Shrine>();
        enemies = new List<Enemy>();
        player = new Player();
        player.Initialize();

        amountOfTurns = 0;

        SpawnLevel();

        // start with other turn
        playersTurn = false;
        othersTurn = false;
    }

    public void Clear()
    {
        // clear objects in list
        while (humans.Count > 0) humans[0].Clear(); 
        humans.Clear();
        humans = null;

        while (barrels.Count > 0) barrels[0].Clear();
        barrels.Clear();
        barrels = null;

        while (shrines.Count > 0) shrines[0].Clear();
        shrines.Clear();
        shrines = null;

        while (enemies.Count > 0) enemies[0].Clear();
        enemies.Clear();
        enemies = null;

        playersTurn = false;
        othersTurn = false;
    }

    public void Update()
    {
        enemies.HandleAction(e => e.ShowStatusEffects());

        if (playersTurn || othersTurn)
            return;

        // start moving enemies
        UberManager.Instance.StartCoroutine(HandleOtherTurn());
    }

    // move creature
    protected IEnumerator HandleOtherTurn()
    {
        othersTurn = true;

        // wait for turn delay
        yield return new WaitForSeconds(delay);

        // handle each enemy
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].StartTurn();

            bool canHandleTurn = !enemies[i].Dead && enemies[i].CurrentActionPoints > 0;

            while (GameManager.Instance.GameOn && canHandleTurn)
            {
                enemies[i].EnemyMove();

                yield return new WaitForSeconds(delay);

                canHandleTurn = !enemies[i].Dead && enemies[i].CurrentActionPoints > 0;
            }

            // need to check for the enemy having killed everything
            if (!GameManager.Instance.GameOn) yield break;

            if (!enemies[i].Dead) enemies[i].EndTurn();

            // need to check for the last enemy died from status effect
            if (!GameManager.Instance.GameOn) yield break;
        }

        // switch turns
        yield return UberManager.Instance.StartCoroutine(BeginPlayerTurn());
    }

    private IEnumerator HandleHumanWalking()
    {
        foreach (Human h in humans)
        {
            // only handle a turn for the human if he is panicking
            if (h.InPanic)
            {
                GameManager.Instance.CameraManager.LockTarget(h.transform);

                // make the human flee as long as he cant
                while (h.CurrentFleePoints > 0)
                {
                    yield return UberManager.Instance.StartCoroutine(h.Flee());
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        yield return null;
    }

    public IEnumerator BeginPlayerTurn()
    {
        // do we have to start goo spawning?
        yield return UberManager.Instance.StartCoroutine(HandleGapSpawning());

        //stop coroutine when goo kills the last human
        if (!GameManager.Instance.GameOn) yield break;

        // make hoomans move
        humans.HandleAction(h => h.StartTurn());
        yield return UberManager.Instance.StartCoroutine(HandleHumanWalking());

        // increase amnt of turns
        amountOfTurns++;

        // show banner
        yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(true));

        // start players turn
        player.StartPlayerTurn();
        shrines.HandleAction(s => s.CheckForActive());

        UIManager.Instance.InGameUI.BeginPlayerTurn();

        GameManager.Instance.CameraManager.UnlockAxis();

        playersTurn = true;
        othersTurn = false;
    }

    public void CheckForExtraAP()
    {
        shrines.HandleAction(s => s.CheckForActive());
    }

    public void EndPlayerMove(int cost = 1, bool endTurn = false)
    {
        if (player.EndPlayerMove(cost, endTurn))
        {
            if (GameManager.Instance.GameOn)
            {
                playersTurn = false;
                GameManager.Instance.TileManager.HidePossibleRoads();

                enemies.HandleAction(e => e.UpdateTarget());
                shrines.HandleAction(s => s.EndPlayerTurn());

                UIManager.Instance.InGameUI.EndPlayerTurn();
                UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(false));
            }
        }
    }

    public void SkipPlayerTurn()
    {
        EndPlayerMove(1, true);
        UIManager.Instance.InGameUI.HideSpellButtons();
        UIManager.Instance.InGameUI.ActivateTeleportButtons(false);
    }

    private IEnumerator HandleGapSpawning()
    {
        if (amountOfTurns == startGapTurn)
            yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.WarningText());
        else if(amountOfTurns > startGapTurn)
            yield return UberManager.Instance.StartCoroutine(SpawnGap());
    }

    private IEnumerator SpawnGap()
    {
        for (int i = 0; i < amountOfTurns - startGapTurn; i++)
        {
            TileNode chosenGap = GameManager.Instance.TileManager.GetPossibleGapNodeReferences();

            GameManager.Instance.CameraManager.LockTarget(chosenGap.Hexagon.transform);

            yield return new WaitForSeconds(delay);

            chosenGap.CreateHexagon(SecTileType.Gap);

            if (!GameManager.Instance.GameOn) break;
        }

        yield return null;
    }

    private void SpawnLevel()
    {
        // spawn nodes
        List<SpawnNode> spawnNodes = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].spawnNodes;

        foreach (SpawnNode s in spawnNodes)
        {
            if (!ContentManager.IsValidSecContentType(s.type, s.secType))
            {
                Debug.LogError("Sectype not supported for given type");
                continue;
            }

            WorldObject toBeSpawned = SpawnFromNode(s);

            if (toBeSpawned == null)
            {
                Debug.LogError("Prefab not found for this spawnnode");
                continue;
            }

            GameManager.Instance.TileManager.SetObject(s.position, toBeSpawned);
        }

        // spawn goo
        List<Coordinate> gooPosses = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].gooStartPosses;
        gooPosses.HandleAction(g => GameManager.Instance.TileManager.GetNodeReference(g).CreateHexagon(SecTileType.Gap));
    }

    private WorldObject SpawnFromNode(SpawnNode s)
    {
        switch (s.type)
        {
            case ContentType.Boss:
                return SpawnBoss(s);

            case ContentType.Minion:
                return SpawnMinion(s);

            case ContentType.Environment:
                return SpawnEnvironment(s);

            case ContentType.Human:
                return SpawnHuman(s);
        }

        return null;
    }

    private WorldObject SpawnHuman(SpawnNode s)
    {
        if (s.secType != SecContentType.Human) return null;

        GameObject prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                                (ContentType.Human, SecContentType.Human)];

        humans.Add(GameObject.Instantiate(prefab, 
                                          GameManager.Instance.TileManager.GetWorldPosition(s.position), 
                                          Quaternion.identity).GetComponent<Human>());
        humans.Last().Initialize(s.position);
        humans.Last().ContractRef = GameManager.Instance.SelectedContracts[humansInstantiated];
        humansInstantiated++;

        return humans.Last();
    }
     
    private WorldObject SpawnEnvironment(SpawnNode s)
    {
        GameObject prefab;

        switch (s.secType)
        {
            case SecContentType.Barrel:
                prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                               (ContentType.Environment, SecContentType.Barrel)];

                barrels.Add(GameObject.Instantiate(prefab,
                                                   GameManager.Instance.TileManager.GetWorldPosition(s.position),
                                                   Quaternion.identity).GetComponent<Barrel>());
                barrels.Last().Initialize(s.position);
                return barrels.Last();

            case SecContentType.Shrine:
                prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                               (ContentType.Environment, SecContentType.Shrine)];

                shrines.Add(GameObject.Instantiate(prefab, 
                                                   GameManager.Instance.TileManager.GetWorldPosition(s.position), 
                                                   Quaternion.identity).GetComponent<Shrine>());
                shrines.Last().Initialize(s.position);
                return shrines.Last();
        }

        return null;
    }

    private WorldObject SpawnMinion(SpawnNode s)
    {
        GameObject prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                               (ContentType.Minion, SecContentType.Wolf)];

        enemies.Add(GameObject.Instantiate(prefab, 
                                           GameManager.Instance.TileManager.GetWorldPosition(s.position), 
                                           Quaternion.identity).GetComponent<Enemy>());
        enemies.Last().Initialize(s.position);

        return enemies.Last();
    }

    private WorldObject SpawnBoss(SpawnNode s)
    {
        GameObject prefab = null;

        switch (s.secType)
        {
            case SecContentType.Arnest:
                prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                                (ContentType.Boss, SecContentType.Arnest)];
                break;
            case SecContentType.Dodin:
                prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                                (ContentType.Boss, SecContentType.Dodin)];
                break;
            case SecContentType.Sketta:
                prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                                (ContentType.Boss, SecContentType.Sketta)];
                break;
        }

        enemies.Add(GameObject.Instantiate(prefab,
                                           GameManager.Instance.TileManager.GetWorldPosition(s.position),
                                           Quaternion.identity).GetComponent<Enemy>());
        enemies.Last().Initialize(s.position);

        return enemies.Last();
    }

    public void RemoveObject(WorldObject toRemove, bool inGame = false)
    {
        if (toRemove.IsBarrel())
        {
            barrels.Remove((Barrel)toRemove);
        }
        else if (toRemove.IsHuman())
        {
            humans.Remove((Human)toRemove);
            if (inGame && humans.Count <= 0) GameManager.Instance.GameOver();
        }
        else if (toRemove.IsMonster())
        {
            enemies.Remove((Enemy)toRemove);
            if (inGame && enemies.Count <= 0) GameManager.Instance.GameOver();
        }
        else if (toRemove.IsShrine())
        {
            shrines.Remove((Shrine)toRemove);
        }

        // finalize object
        Remove(toRemove);
    }

    private void Remove(WorldObject toRemove)
    {
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove);
        GameObject.Destroy(toRemove.gameObject);
    }
}
