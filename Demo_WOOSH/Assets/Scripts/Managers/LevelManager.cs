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
        humans = new List<Human>();
        barrels = new List<Barrel>();
        shrines = new List<Shrine>();
        enemies = new List<Enemy>();
        player = new Player();

        SpawnLevel();

        // start with player turn
        //playersTurn = true;
        //BeginPlayerTurn();
        //othersTurn = false;
    }

    public void Update()
    {
        if (playersTurn || othersTurn)
            return;

        // start moving enemies
        GameManager.Instance.StartCoroutine(HandleOtherTurn());
    }

    public IEnumerator BeginPlayerTurn()
    {
        // increase amnt of turns
        amountOfTurns++;

        // do we have to start goo spawning?
        yield return GameManager.Instance.StartCoroutine(CheckForGooSpawning());

        // show banner
        yield return GameManager.Instance.StartCoroutine(GameManager.Instance.UiManager.StartTurn(true));

        // count extra actionpoints
        shrines.HandleAction(s => s.CheckForActive());
        int extraPoints = 0;
        for (int i = 0; i < shrines.Count; i++) extraPoints += shrines[i].Active ? 1 : 0;
        
        humans.HandleAction(h => h.DecreaseInvisiblePoints());

        // start players turn
        player.StartPlayerTurn(extraPoints);

        GameManager.Instance.UiManager.BeginPlayerTurn();

        playersTurn = true;
        othersTurn = false;
    }

    public void EndPlayerMove(int cost = 1, bool endTurn = false)
    {
        if (player.EndPlayerMove(cost, endTurn))
        {
            enemies.HandleAction(e => e.UpdateTarget());

            playersTurn = false;
            GameManager.Instance.UiManager.EndPlayerTurn();
            GameManager.Instance.StartCoroutine(GameManager.Instance.UiManager.StartTurn(false));
        }

        //apText.text = currentActionPoints + "";
    }

    private IEnumerator CheckForGooSpawning()
    {
        if (amountOfTurns == 4)
        {
            yield return GameManager.Instance.StartCoroutine(GameManager.Instance.UiManager.WarningText());
        }

        if (amountOfTurns > 4)
        {
            yield return GameManager.Instance.StartCoroutine(SpawnGoo());
        }
    }

    private IEnumerator SpawnGoo()
    {
        List<TileNode> possGooNodes = GameManager.Instance.TileManager.GetPossibleGooNodeReferences();
        int rnd = UnityEngine.Random.Range(0, possGooNodes.Count);
        TileNode chosenGoo = possGooNodes[rnd];

        chosenGoo.Content.SetTileType(TileManager.TileType.Goo);

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
            while (e.CurrentActionPoints > 0)
            {
                // make creature move
                e.EnemyMove();

                // delay
                yield return new WaitForSeconds(moveDelay);
            }
            e.EndTurn();
        }

        // switch turns

        yield return GameManager.Instance.StartCoroutine(BeginPlayerTurn());

        //yield return new WaitForSeconds(turnDelay);
    }

    private void SpawnLevel()
    {
        // spawn nodes
        List<SpawnNode> spawnNodes = ContentManager.Instance.Levels[0].spawnNodes;

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
                        GameObject.Instantiate(ContentManager.Instance.Humans[0], spawnPosition, Quaternion.identity)
                            .GetComponent<Human>());
                    humans.Last().Initialize(s.position);
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
        GameManager.Instance.TileManager.GetNodeReference(ContentManager.Instance.Levels[0].gooStartPos).Content.SetTileType(TileManager.TileType.Goo);
    }

    //TODO: refactor!
    public void RemoveHuman(Human toRemove)
    {
        humans.Remove(toRemove);

        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove.Type);
        GameObject.Destroy(toRemove.gameObject);

        //if (humans.Count <= 0)
        //{
        //    Application.LoadLevel("Lose");
        //}

        shrines.HandleAction(s => s.CheckForActive());
    }

    public void RemoveShrine(Shrine toRemove)
    {
        shrines.Remove(toRemove);
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove.Type);
        GameObject.Destroy(toRemove.gameObject);
    }

    public void RemoveBarrel(Barrel toRemove)
    {
        barrels.Remove(toRemove);
        GameManager.Instance.TileManager.RemoveObject(toRemove.GridPosition, toRemove.Type);
        GameObject.Destroy(toRemove.gameObject);
    }
}
