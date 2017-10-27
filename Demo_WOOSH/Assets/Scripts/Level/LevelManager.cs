using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private List<WorldObject> removedObjects = new List<WorldObject>();
    public List<WorldObject> RemovedObjects { get { return removedObjects; } }

    private Player player;
    public Player Player { get { return player; } }

    private bool playersTurn = false;
    private bool othersTurn = false;
    public bool PlayersTurn { get { return playersTurn; } }
    private int extraPoints;

    private int amountOfTurns = 1;
    public int AmountOfTurns { get { return amountOfTurns; } }

    private float delay = 0.5f;
    private int startGapTurn = 2;

    public int StartGapTurn
    {
        set
        {
            if(!UberManager.Instance.DevelopersMode) return;
            startGapTurn = value;
        }
    }

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
        removedObjects = new List<WorldObject>();
        player = new Player();
        player.Initialize();

        amountOfTurns = 1;
        
        if (UberManager.Instance.DevelopersMode) SpawnEmptyLevel();
        else SpawnLevel();

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

        while (removedObjects.Count > 0) removedObjects[0].Clear();
        removedObjects.Clear();
        removedObjects = null;

        playersTurn = false;
        othersTurn = false;
    }

    public void RestartDEVMODE()
    {
        humans = new List<Human>();
        barrels = new List<Barrel>();
        shrines = new List<Shrine>();
        enemies = new List<Enemy>();
        removedObjects = new List<WorldObject>();
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

        // handle each enemy
        for (int i = 0; i < enemies.Count; i++)
        {
            // wait for turn delay
            yield return new WaitForSeconds(delay);

            if(GameManager.Instance.Paused) yield break;

            Enemy e = enemies[i];
            e.StartTurn();

            bool canHandleTurn = GameManager.Instance.GameOn && !e.Dead && e.CurrentActionPoints > 0;

            while (canHandleTurn)
            {
                e.EnemyMove();

                yield return new WaitForSeconds(delay);

                if (GameManager.Instance.Paused) break;

                canHandleTurn = GameManager.Instance.GameOn && !e.Dead && e.CurrentActionPoints > 0;
            }

            // need to check for the enemy having killed everything
            if (!GameManager.Instance.GameOn) yield break;

            if (!e.Dead) e.EndTurn();
            if (e.Dead)
            {
                i--;
            }

            if (GameManager.Instance.Paused)
            {
                playersTurn = false;
                othersTurn = false;
                yield break;
            }

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
            if (GameManager.Instance.Paused) yield break;

            // only handle a turn for the human if he is panicking
            if (h.InPanic)
            {
                GameManager.Instance.CameraManager.LockTarget(h.transform);

                // make the human flee as long as he cant
                while (h.CurrentFleePoints > 0)
                {
                    yield return UberManager.Instance.StartCoroutine(h.Flee());
                    if (GameManager.Instance.Paused) break;

                    yield return new WaitForSeconds(delay);
                }
            }
        }
        shrines.HandleAction(s => s.CheckForActive(false));
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
        if(!GameManager.Instance.Paused) yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(true));

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
        if (GameManager.Instance.TileManager.GetNodeWithGapReferences().Count <= 0) yield break;

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

            if (!GameManager.Instance.GameOn || GameManager.Instance.Paused) break;
        }

        yield return null;
    }

    private void SpawnEmptyLevel()
    {
        
    }

    public void SpawnObjectDEVMODE(SpawnNode s)
    {
        if (!UberManager.Instance.DevelopersMode) return;

        if (!ContentManager.IsValidSecContentType(s.type, s.secType))
        {
            Debug.LogError("Sectype not supported for given type");
            return;
        }

        WorldObject toBeSpawned = null;

        switch (s.type)
        {
            case ContentType.Boss:
                toBeSpawned = SpawnBoss(s);
                break;
            case ContentType.Minion:
                toBeSpawned = SpawnMinion(s);
                break;
            case ContentType.Environment:
                toBeSpawned = SpawnEnvironment(s);
                break;
            case ContentType.Human:
                toBeSpawned = SpawnHuman(s, false);
                break;
        }

        if (toBeSpawned == null)
        {
            Debug.LogError("Prefab not found for this spawnnode");
            return;
        }

        GameManager.Instance.TileManager.SetObject(s.position, toBeSpawned);
    }

    public void SpawnLevelDEVMODE(List<SpawnNode> spawnNodes)
    {
        // spawn nodes
        foreach (SpawnNode s in spawnNodes)
        {
            if (!ContentManager.IsValidSecContentType(s.type, s.secType))
            {
                Debug.LogError("Sectype not supported for given type");
                continue;
            }

            WorldObject toBeSpawned = null;

            switch (s.type)
            {
                case ContentType.Boss:
                    toBeSpawned = SpawnBoss(s);
                    break;
                case ContentType.Minion:
                    toBeSpawned = SpawnMinion(s);
                    break;
                case ContentType.Environment:
                    toBeSpawned = SpawnEnvironment(s);
                    break;
                case ContentType.Human:
                    toBeSpawned = SpawnHuman(s, false);
                    break;
            }

            if (toBeSpawned == null)
            {
                Debug.LogError("Prefab not found for this spawnnode");
                return;
            }

            GameManager.Instance.TileManager.SetObject(s.position, toBeSpawned);
        }
    }

    private void SpawnLevel()
    {
        // spawn nodes
        List<SpawnNode> spawnNodes = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).spawnNodes;

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

        startGapTurn = ContentManager.Instance.LevelData(GameManager.Instance.CurrentLevel).dangerStartGrow;
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

    private WorldObject SpawnHuman(SpawnNode s, bool exitstingContract = true)
    {
        if (!exitstingContract && !UberManager.Instance.DevelopersMode)
        {
            Debug.Log("You have to assign a contract in game mode");
            exitstingContract = true;
        }

        if (s.secType != SecContentType.Human) return null;

        GameObject prefab = ContentManager.Instance.ContentPrefabs[new KeyValuePair<ContentType, SecContentType>
                                                                (ContentType.Human, SecContentType.Human)];

        humans.Add(GameObject.Instantiate(prefab, 
                                          GameManager.Instance.TileManager.GetWorldPosition(s.position), 
                                          Quaternion.identity).GetComponent<Human>());
        humans.Last().Initialize(s.position);
        if(exitstingContract) humans.Last().ContractRef = GameManager.Instance.SelectedContracts[humansInstantiated];
        else humans.Last().ContractRef = UberManager.Instance.ContractManager.GenerateRandomContract(null);
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

    public void RemoveObject(WorldObject toRemove, bool fromEditor = false)
    {
        if (UberManager.Instance.DevelopersMode && !fromEditor)
        {
            RemoveObjectDEVMODE(toRemove);
            return;
        }

        if (toRemove.IsBarrel())
        {
            barrels.Remove((Barrel)toRemove);
        }
        else if (toRemove.IsHuman())
        {
            humans.Remove((Human)toRemove);
            if (!fromEditor && GameManager.Instance.GameOn && humans.Count <= 0)
            {
                Remove(toRemove);
                GameManager.Instance.GameOver();
                return;
            }
            else
            {
                Remove(toRemove);
                shrines.HandleAction(s => s.CheckForActive(false));
                return;
            }
        }
        else if (toRemove.IsMonster())
        {
            enemies.Remove((Enemy)toRemove);
            if (!fromEditor && GameManager.Instance.GameOn && enemies.Count <= 0)
            {
                IncreaseHappinessOfSurvivors();

                Remove(toRemove);
                GameManager.Instance.GameOver();
                return;
            }
        }
        else if (toRemove.IsShrine())
        {
            shrines.Remove((Shrine)toRemove);
        }

        // finalize object
        Remove(toRemove);
    }

    //increase happiness of all surviving humans
    private void IncreaseHappinessOfSurvivors()
    {
        if (humans.Count > 0)
        {
            for (int i = 0; i < humans.Count; i++)
            {
                humans[i].ContractRef.MakeHappy();
                Debug.Log("Mii so happy :D");
            }
        }
    }

    private void RemoveObjectDEVMODE(WorldObject toRemove)
    {
        removedObjects.Add(toRemove);
        removedObjects.Last().gameObject.SetActive(false);
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove);

        if (toRemove.IsBarrel())
        {
            barrels.Remove((Barrel)toRemove);
        }
        else if (toRemove.IsHuman())
        {
            humans.Remove((Human)toRemove);
            if (GameManager.Instance.GameOn && humans.Count <= 0)
            {
                GameManager.Instance.GameOver();
                return;
            }
            else
            {
                shrines.HandleAction(s => s.CheckForActive(false));
                return;
            }
        }
        else if (toRemove.IsMonster())
        {
            enemies.Remove((Enemy)toRemove);
            if (GameManager.Instance.GameOn && enemies.Count <= 0)
            {
                GameManager.Instance.GameOver();
                return;
            }
        }
        else if (toRemove.IsShrine())
        {
            shrines.Remove((Shrine)toRemove);
        }
    }

    private void Remove(WorldObject toRemove)
    {
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove);
        GameObject.Destroy(toRemove.gameObject);
    }

    public void ResetAllDEVMODE()
    {
        humans.HandleAction(h => h.Reset());
        barrels.HandleAction(b => b.Reset());
        shrines.HandleAction(s => s.Reset());
        enemies.HandleAction(e => e.Reset());
        removedObjects.HandleAction(o => o.Reset());
    }

    public void ResetAllToInitDEVMODE(List<SpawnNode> spawnNodes)
    {
        // copy the spawnnodes, so we can be sure we had them all 
        List<SpawnNode> copyList = new List<SpawnNode>(spawnNodes);

        // copy all to be resetted objects to a list
        List<WorldObject> allObjects = new List<WorldObject>();
        allObjects.AddRange(humans.Cast<WorldObject>());
        allObjects.AddRange(barrels.Cast<WorldObject>());
        allObjects.AddRange(shrines.Cast<WorldObject>());
        allObjects.AddRange(enemies.Cast<WorldObject>());
        allObjects.AddRange(removedObjects);

        SpawnNode s;
        WorldObject w;

        for (int i = 0; i < copyList.Count; i++)
        {
            for (int j = 0; j < allObjects.Count; j++)
            {
                s = copyList[i];
                w = allObjects[j];

                if (s.secType == w.Type)
                {
                    //if in removed objects, add
                    if (removedObjects.Contains(w))
                    {
                        //TODO: switch to add back to update lists
                        switch (ContentManager.GetPrimaryFromSecContent(w.Type))
                        {
                            case ContentType.Boss:
                            case ContentType.Minion:
                                enemies.Add((Enemy)w);
                                break;
                            case ContentType.Environment:
                                if (w.Type == SecContentType.Barrel)
                                    barrels.Add((Barrel) w);
                                else if(w.Type == SecContentType.Shrine)
                                    shrines.Add((Shrine)w);
                                else
                                    Debug.LogError("Tried to add a non-type to levelmanager update lists " + w.name);
                                break;
                            case ContentType.Human:
                                humans.Add((Human)w);
                                break;
                            default:
                                Debug.LogError("Tried to add a non-type to levelmanager update lists " + w.name);
                                break;
                        }
                        removedObjects.Remove(w);
                        GameManager.Instance.TileManager.SetObject(s.position, w);
                    }
                    //else move
                    else
                    {
                        GameManager.Instance.TileManager.MoveObject(w.GridPosition, s.position, w);
                    }

                    w.gameObject.transform.position = GameManager.Instance.TileManager.GetWorldPosition(s.position);
                    w.ResetToInitDEVMODE(s.position);

                    copyList.Remove(s);
                    allObjects.Remove(w);

                    i--;
                    break;
                }
            }
        }

        if (copyList.Count > 0)
        {
            Debug.LogError("More spawnnodes than existing objects found!");
        }
        if(allObjects.Count > 0)
        {
            Debug.LogError("Not enough spawnnodes, not all objects resetted");
        }
    }

    public void ClearRemoveObjectsDEVMODE()
    {
        while (removedObjects.Count > 0)
        {
            GameObject.Destroy(removedObjects.Last().gameObject);
            removedObjects.Remove(removedObjects.Last());
        }

        removedObjects.Clear();
        removedObjects = null;
    }

    public void ResetTurnAmount()
    {
        amountOfTurns = 0;
    }

    public void ResetTurns()
    {
        amountOfTurns = 0;
        playersTurn = false;
        othersTurn = false;
    }
}
