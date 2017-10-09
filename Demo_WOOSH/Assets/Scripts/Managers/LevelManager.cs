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

    private bool playersTurn = false;
    private bool othersTurn = false;
    private int amountOfTurns = 0;

    public bool PlayersTurn
    {
        get { return playersTurn; }
    }

    private float turnDelay = 0.8f;
    private float moveDelay = 1f;

    public void Initialize()
    {
        SetUpLevel();
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
        playersTurn = true;
        othersTurn = false;
        UberManager.Instance.StartCoroutine(BeginPlayerTurn());
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
        if (playersTurn || othersTurn)
            return;

        // start moving enemies
        UberManager.Instance.StartCoroutine(HandleOtherTurn());
    }

    public IEnumerator BeginPlayerTurn()
    {
        // increase amnt of turns
        amountOfTurns++;

        // show banner
        yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(true));

        // count extra actionpoints
        shrines.HandleAction(s => s.CheckForActive());
        int extraPoints = 0;
        for (int i = 0; i < shrines.Count; i++) extraPoints += shrines[i].Active ? 1 : 0;
        
        humans.HandleAction(h => h.DecreaseInvisiblePoints());

        // start players turn
        player.StartPlayerTurn(extraPoints);

        // do we have to start goo spawning?
        yield return UberManager.Instance.StartCoroutine(CheckForGooSpawning());

        UIManager.Instance.InGameUI.BeginPlayerTurn();

        GameManager.Instance.CameraManager.UnlockAxis();

        playersTurn = true;
        othersTurn = false;
    }

    public void EndPlayerMove(int cost = 1, bool endTurn = false)
    {
        if (player.EndPlayerMove(cost, endTurn))
        {
            if (GameManager.Instance.GameOn)
            {
                enemies.HandleAction(e => e.UpdateTarget());

                playersTurn = false;
                GameManager.Instance.TileManager.HidePossibleRoads();

                UIManager.Instance.InGameUI.EndPlayerTurn();
                UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.StartTurn(false));
            }
        }

        //apText.text = currentActionPoints + "";
    }

    private IEnumerator CheckForGooSpawning()
    {
        if (amountOfTurns == 4)
        {
            yield return UberManager.Instance.StartCoroutine(UIManager.Instance.InGameUI.WarningText());
        }

        if (amountOfTurns > 4)
        {
            yield return UberManager.Instance.StartCoroutine(SpawnGoo());
        }
    }
    private IEnumerator SpawnGoo()
    {
        for (int i = 0; i < amountOfTurns - 4; i++)
        {
            List<TileNode> possGooNodes = GameManager.Instance.TileManager.GetPossibleGooNodeReferences();
            int rnd = UnityEngine.Random.Range(0, possGooNodes.Count);
            TileNode chosenGoo = possGooNodes[rnd];

            chosenGoo.Content.SetTileType(TileManager.TileType.Goo);
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

        foreach (Enemy e in enemies)
        {
            e.StartTurn();

            GameManager.Instance.CameraManager.LockTarget(e.transform);

            while (e.CurrentActionPoints > 0)
            {
                // make creature move
                e.EnemyMove();

                if (!GameManager.Instance.GameOn) break;

                // delay
                yield return new WaitForSeconds(moveDelay);
            }

            if (!GameManager.Instance.GameOn) break;

            e.EndTurn();
        }

        // switch turns

        if (!GameManager.Instance.GameOn) yield return null;
        else yield return UberManager.Instance.StartCoroutine(BeginPlayerTurn());

        //yield return new WaitForSeconds(turnDelay);
    }

    private void SpawnLevel()
    {
        // spawn nodes
        List<SpawnNode> spawnNodes = ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].spawnNodes;

        int humansInstantiated = 0;

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
                    //TODO: difference between monsters 
                    enemies.Add(
                        GameObject.Instantiate(ContentManager.Instance.Bosses[0], spawnPosition, Quaternion.identity)
                            .GetComponent<Enemy>());
                    enemies.Last().Initialize(s.position);
                    break;
            }
        }

        // spawn goo
        GameManager.Instance.TileManager.GetNodeReference(ContentManager.Instance.LevelDataContainer.LevelData[GameManager.Instance.CurrentLevel].gooStartPos).Content.SetTileType(TileManager.TileType.Goo);
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
