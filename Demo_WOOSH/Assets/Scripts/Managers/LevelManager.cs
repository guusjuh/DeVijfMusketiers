using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private List<Human> humans;
    private List<Barrel> barrels;
    private List<Shrine> shrines;
    private List<Enemy> enemies;

    private bool playersTurn = false;
    private bool othersTurn = false;
    private int amountOfTurns = 0;

    private float turnDelay = 0.8f;
    private float moveDelay = 1f;

    public void Initialize()
    {
        humans = new List<Human>();
        barrels = new List<Barrel>();
        shrines = new List<Shrine>();
        enemies = new List<Enemy>();

        SpawnLevel();

        // start with player turn
        //playersTurn = true;
        //BeginPlayerTurn();
        //othersTurn = false;
    }

    public void Update() {
        if (playersTurn || othersTurn)
            return;

        // start moving enemies
        GameManager.Instance.StartCoroutine(HandleOtherTurn());
    }

    public void BeginPlayerTurn()
    {

    }

    public void EndPlayerTurn()
    {

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
        playersTurn = true;
        BeginPlayerTurn();
        othersTurn = false;
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
                    barrels.Add(GameObject.Instantiate(ContentManager.Instance.Barrel, spawnPosition, Quaternion.identity).GetComponent<Barrel>());
                    barrels.Last().Initialize(s.position);
                    break;
                case TileManager.ContentType.Human:
                    humans.Add(GameObject.Instantiate(ContentManager.Instance.Humans[0], spawnPosition, Quaternion.identity).GetComponent<Human>());
                    humans.Last().Initialize(s.position);
                    break;
                case TileManager.ContentType.Shrine:
                    shrines.Add(GameObject.Instantiate(ContentManager.Instance.Shrine, spawnPosition, Quaternion.identity).GetComponent<Shrine>());
                    shrines.Last().Initialize(s.position);
                    break;
                case TileManager.ContentType.WalkingMonster:
                    //TODO: difference between monsters 
                    enemies.Add(GameObject.Instantiate(ContentManager.Instance.Bosses[0], spawnPosition, Quaternion.identity).GetComponent<Enemy>());
                    enemies.Last().Initialize(s.position);
                    break;
            }
        }

        // spawn goo
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
