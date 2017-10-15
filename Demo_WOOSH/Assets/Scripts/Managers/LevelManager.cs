using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
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

    //TODO: implement hooman & goo turns
    private bool playersTurn = false;
    private bool othersTurn = false;

    private int amountOfTurns = 0;
    public int AmountOfTurns { get { return amountOfTurns; } }
    private int extraPoints;

    private bool init = false;
    public bool Initialized { get { return init; } }

    public bool PlayersTurn
    {
        get { return playersTurn; }
    }

    private float turnDelay = 0.5f;
    private float moveDelay = 0.5f;
    private float gapDelay = 0.5f;

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
        humans = new List<Human>();
        barrels = new List<Barrel>();
        shrines = new List<Shrine>();
        enemies = new List<Enemy>();
        player = new Player();
        player.Initialize();

        amountOfTurns = 0;

        SpawnLevel();

        // start with player turn
        playersTurn = false;
        othersTurn = false;

        //UberManager.Instance.StartCoroutine(BeginPlayerTurn());
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

    public IEnumerator BeginPlayerTurn()
    {
        // do we have to start goo spawning?
        yield return UberManager.Instance.StartCoroutine(CheckForGapSpawning());

        //stop coroutine when goo kills the last human
        if (!GameManager.Instance.GameOn) yield break;

        // make hoomans move
        humans.HandleAction(h => h.StartTurn());
        yield return UberManager.Instance.StartCoroutine(CheckForHumanWalking());

        // increase amnt of turns
        amountOfTurns++;

        //stop coroutine when goo kills the last human
        if (!GameManager.Instance.GameOn) yield break;

        // count extra actionpoints
        shrines.HandleAction(s => s.CheckForActive());
        extraPoints = 0;
        for (int i = 0; i < shrines.Count; i++) extraPoints += shrines[i].Active ? 1 : 0;
        
        // show banner
        yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(true));

        // start players turn
        player.StartPlayerTurn(extraPoints);

        UIManager.Instance.InGameUI.BeginPlayerTurn();

        GameManager.Instance.CameraManager.UnlockAxis();

        playersTurn = true;
        othersTurn = false;
    }

    public void CheckForExtraAP()
    {
        shrines.HandleAction(s => s.CheckForActive());
        int extraPoints = 0;
        for (int i = 0; i < shrines.Count; i++) extraPoints += shrines[i].Active ? 1 : 0;
        
        //check for increase in points
        int diff = extraPoints - this.extraPoints;
        if (diff > 0)
        {
            player.IncreaseActionPoints(diff);
            this.extraPoints = extraPoints;
        }
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

                UIManager.Instance.InGameUI.EndPlayerTurn();
                UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(false));
            }
        }
    }

    private IEnumerator CheckForHumanWalking()
    {
        //dont walk the very first turn
        //if (amountOfTurns == 0) yield break;

        foreach (Human h in humans)
        {
            // only handle a turn for the human if he is panicking
            if (h.InPanic) {
                GameManager.Instance.CameraManager.LockTarget(h.transform);

                // make the human flee as long as he cant
                while (h.CurrentFleePoints > 0) {
                    yield return UberManager.Instance.StartCoroutine(h.Flee());
                    yield return new WaitForSeconds(moveDelay);
                }
            }
        }

        yield return null;
    }

    private IEnumerator CheckForGapSpawning()
    {
        if (amountOfTurns == 2)
        {
            yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.WarningText());
        }

        if (amountOfTurns > 2)
        {
            yield return UberManager.Instance.StartCoroutine(SpawnGap());
        }
    }
    private IEnumerator SpawnGap()
    {
        for (int i = 0; i < amountOfTurns - 2; i++)
        {
            List<TileNode> possGapNodes = GameManager.Instance.TileManager.GetPossibleGapNodeReferences();
            int rnd = UnityEngine.Random.Range(0, possGapNodes.Count);
            TileNode chosenGap = possGapNodes[rnd];

            GameManager.Instance.CameraManager.LockTarget(chosenGap.Hexagon.transform);
            yield return new WaitForSeconds(gapDelay);

            chosenGap.Content.SetTileType(TileManager.TileType.Gap);

            if (!GameManager.Instance.GameOn) break;
        }

        yield return null;
    }

    // move creature
    protected IEnumerator HandleOtherTurn()
    {
        //Debug.Log("Handling creature turn");
        othersTurn = true;

        // wait for turn delay
        yield return new WaitForSeconds(turnDelay);

        for (int i = 0; i < enemies.Count; i++) {
            enemies[i].StartTurn();

            GameManager.Instance.CameraManager.LockTarget(enemies[i].transform);

            while (GameManager.Instance.GameOn && !enemies[i].Dead && enemies[i].CurrentActionPoints > 0)
            {
                // make creature move
                enemies[i].EnemyMove();

                // delay
                yield return new WaitForSeconds(moveDelay);
            }

            // need to check for the enemy having killed everything
            if (!GameManager.Instance.GameOn) yield break;

            if (!enemies[i].Dead) enemies[i].EndTurn();
            //else i--;

            // need to check for the last enemy died from status effect
            if (!GameManager.Instance.GameOn) yield break;
        }

        // switch turns

        if (!GameManager.Instance.GameOn) yield break;
        else yield return UberManager.Instance.StartCoroutine(BeginPlayerTurn());

        //yield return new WaitForSeconds(turnDelay);
    }

    private void SpawnLevel()
    {
        // spawn nodes
        List<SpawnNode> spawnNodes = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].spawnNodes;

        int humansInstantiated = 0;
        bool bossSpawned = false;

        foreach (SpawnNode s in spawnNodes)
        {
            Vector2 spawnPosition = GameManager.Instance.TileManager.GetWorldPosition(s.position);
            GameManager.Instance.TileManager.SetObject(s.position, s.type);

            //TODO: need more specific type of content definition
            // for example: content type for tiles and occupation, content for the actual prefab and different enemies and stuff
            switch (s.type)
            {
                case TileManager.ContentType.Barrel:
                    barrels.Add(
                        GameObject.Instantiate(ContentManager.Instance.Barrel, spawnPosition, Quaternion.identity)
                            .GetComponent<Barrel>());
                    barrels.Last().Initialize(s.position);
                    break;
                case TileManager.ContentType.Human:
                    humans.Add(
                        GameObject.Instantiate(ContentManager.Instance.Human, spawnPosition, Quaternion.identity)
                            .GetComponent<Human>());
                    humans.Last().Initialize(s.position);
                    humans.Last().ContractRef = GameManager.Instance.SelectedContracts[humansInstantiated];
                    humansInstantiated++;
                    break;
                case TileManager.ContentType.Shrine:
                    shrines.Add(
                        GameObject.Instantiate(ContentManager.Instance.Shrine, spawnPosition, Quaternion.identity)
                            .GetComponent<Shrine>());
                    shrines.Last().Initialize(s.position);
                    break;
                case TileManager.ContentType.WalkingMonster:
                case TileManager.ContentType.FlyingMonster:
                    if (!bossSpawned)
                    {
                        enemies.Add(
                            GameObject.Instantiate(
                                    ContentManager.Instance.Bosses[
                                        ContentManager.Instance.LevelDataContainer.LevelData[
                                            GameManager.Instance.CurrentLevel].bossID], spawnPosition,
                                    Quaternion.identity)
                                .GetComponent<Enemy>());
                        bossSpawned = true;
                    }
                    else
                    {
                        enemies.Add(GameObject.Instantiate(ContentManager.Instance.Minions[0], spawnPosition, Quaternion.identity).GetComponent<Enemy>());
                    }
                    
                    enemies.Last().Initialize(s.position);
                    break;
            }
        }

        // spawn goo
        List<Coordinate> gooPosses = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].gooStartPosses;
        gooPosses.HandleAction(g => GameManager.Instance.TileManager.GetNodeReference(g).Content.SetTileType(TileManager.TileType.Gap));
    }

    //TODO: refactor!
    public void RemoveHuman(Human toRemove, bool inGame = false)
    {
        humans.Remove(toRemove);

        Remove(toRemove);

        if (inGame && humans.Count <= 0)
        {
            //TODO: game over
            Debug.Log("You lost!");
            GameManager.Instance.GameOver();
        }

        shrines.HandleAction(s => s.CheckForActive());
    }

    public void RemoveShrine(Shrine toRemove)
    {
        shrines.Remove(toRemove);
        Remove(toRemove);
    }

    public void RemoveBarrel(Barrel toRemove)
    {
        barrels.Remove(toRemove);
        Remove(toRemove);
    }

    public void RemoveEnemy(Enemy toRemove, bool inGame = false)
    {
        enemies.Remove(toRemove);
        Remove(toRemove);

        if (inGame && enemies.Count <= 0)
        {
            //TODO: win!
            Debug.Log("You won!");
            GameManager.Instance.GameOver();
        }
    }

    private void Remove(WorldObject toRemove)
    {
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove.Type);
        GameObject.Destroy(toRemove.gameObject);
    }
}
