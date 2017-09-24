using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : BaseEnemy
{
    private int spawnCooldown = 0;
    private int totalSpawnCooldown = 3;

    private GameObject minionPrefab;
    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };


    // initialize this instance
    public override void Initialize(int x, int y)
    {
        base.Initialize(x, y);

        minionPrefab = Resources.Load<GameObject>("Prefabs/Minion");

        totalActionPoints = 3;
        health = 100;

        GameManager.Instance.LevelManager.TileMap.SetObject(x, y, TileMap.Types.Monster);

        // select the first target to destory! mwoehahaha
        SelectTarget();

        if (currentPath.Count <= 2) target.Targeted = true;
    }

    public override void MoveEnemy()
    {
        // do spawn minion attack
        if (currentActionPoints >= 3 && spawnCooldown <= 0 && GameManager.Instance.Creatures.Count <= 0)
        {
            SpawnMinions();
            return;
        }

        //TODO: targets
        if (target == null)
        {
            SelectTarget();
        }
        if (!CheckTargetForSuperSafe()) return;
        UpdateTarget();
        if (!CheckTargetForSuperSafe()) return;

        // do raycast to check for world objects
        RaycastHit2D hit;

        int xDir = currentPath[1].x - x;
        int yDir = currentPath[1].y - y;

        // can I move? 
        bool canMove = CanMove(xDir, yDir, out hit);

        // either barrel on my way or target reached
        if (!canMove)
        {
            // target reached
            if (currentPath.Count <= 2)
            {
                if (hit.transform.gameObject.transform == target.transform) OnCantMove(target);

                // set the target to null so the next turn, searching for new target
                target = null;
            }
            // barrel in my way
            else
            {
                if (hit.transform.GetComponent<Barrel>() != null) OnCantMove(hit.transform.GetComponent<Barrel>());
            }
        }
        // else: move
        else
        {
            // update pos in tile map
            GameManager.Instance.LevelManager.TileMap.MoveObject(x, y, x + xDir, y + yDir, TileMap.Types.Monster);

            // update x and y
            x += xDir;
            y += yDir;

            // perform move
            StartCoroutine(SmoothMovement(new Vector2(transform.position.x, transform.position.y) + new Vector2(xDir, yDir)));

            // remove current path[0], e.g. node i was standing on
            currentPath.RemoveAt(0);

            // update target if needed
            if (currentPath.Count <= 2) target.Targeted = true; 
        }

        // lose one action point
        if (currentActionPoints != 0)
        {
            currentActionPoints--;
        }
    }

    protected bool CanMove(int xDir, int yDir, out RaycastHit2D hit)
    {
        //Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;

        // Calculate end position based on the direction parameters passed in when calling Move.
        Vector2 end = start + new Vector2(xDir, yDir);

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);

        //Re-enable boxCollider after linecast
        boxCollider.enabled = true;

        //Check if anything was hit
        if (hit.transform == null)
        {
            //Return true to say that it can move
            return true;
        }

        //If something was hit, return false, cannot move.
        return false;
    }

    public override void Hit(int dmg)
    {
        base.Hit(50);

        if (health <= 0)
        {
            GameManager.Instance.BossDead();
            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(HitVisual());
        }
    }

    private void SpawnMinions()
    {
        spawnCooldown = totalSpawnCooldown;

        List<Vector3> availablePositions = new List<Vector3>();

        for (int i = 0; i < positions.Length; i++)
        {
            if (GameManager.Instance.LevelManager.TileMap.CanSpawnMinion(x + (int)positions[i].x, y + (int)positions[i].y))
            {
                availablePositions.Add(new Vector3(x + (int)positions[i].x, y + (int)positions[i].y, 0));
            }
        }

        if (availablePositions.Count > 0)
        {
            int selected = 0;
            Debug.Log(availablePositions.Count);
            for (int i = 0; i < (availablePositions.Count >= 2 ? 2 : 1); i++)
            {
                selected = UnityEngine.Random.Range(0, availablePositions.Count);

                GameObject go = GameObject.Instantiate(minionPrefab, availablePositions[selected], Quaternion.identity);
                go.GetComponent<Minion>().Initialize((int)availablePositions[selected].x, (int)availablePositions[selected].y);

                availablePositions.RemoveAt(selected);
            }
        }
        else
        {
            Debug.LogError("Boss is super stuck!!");
        }

        currentActionPoints = 0;
    }


    public void EndPlayerTurn()
    {
        spawnCooldown--;
    }
}
