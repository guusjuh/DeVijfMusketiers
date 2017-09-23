using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : BaseEnemy
{
    // initialize this instance
    public override void Initialize(int x, int y)
    {
        base.Initialize(x, y);

        totalActionPoints = 1;
        health = 1;

        GameManager.Instance.LevelManager.TileMap.SetBat(x, y);

        // select the first target to destory! mwoehahaha
        SelectTarget();

        if (currentPath.Count <= 2) target.Targeted = true;

        GameManager.Instance.AddCreature(this);
    }

    public override void MoveEnemy()
    {
        //TODO: targets
        if (target == null)
        {
            SelectTarget();
        }
        if (currentPath == null || target == null || prevTarget == null)
        {
            if (currentPath == null && target == null && prevTarget == null)
            {
                currentActionPoints = 0;
                Debug.Log("Minion: skipped a move: no target or route found");
                return;
            }
            Debug.LogError("Minion: Wubba lubba dup dup");
            return;
        }

        UpdateTarget();

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
            else if(hit.transform.GetComponent<Barrel>() != null)
            {
                // update pos in tile map
                GameManager.Instance.LevelManager.TileMap.MoveBat(x, y, x + xDir, y + yDir);

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
        }
        // else: move
        else
        {
            // update pos in tile map
            GameManager.Instance.LevelManager.TileMap.MoveBat(x, y, x + xDir, y + yDir);

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
        currentActionPoints--;
    }

    // NOTE: this thing returns false if barrel in the way, but bats don't care and go over it
    protected bool CanMove(int xDir, int yDir, out RaycastHit2D hit)
    {
        //Store start position to move from, based on objects current transform position.
        Vector2 start = transform.position;

        // Calculate end position based on the direction parameters passed in when calling Move.
        Vector2 end = start + new Vector2(xDir, yDir);

        // check tiletype waar ik op sta
        bool standingOn = !GameManager.Instance.LevelManager.TileMap.Empty(x, y, false);
        GameObject goToReset = null;

        // change layer of object we're standing on to do raycast for next tile
        if (standingOn)
        {
            boxCollider.enabled = false;

            RaycastHit2D tempHit = Physics2D.Linecast(start, end, blockingLayer);


            // found the object we're standing on
            if (tempHit.transform != null)
            {
                if ((tempHit.transform.position - transform.position).magnitude > 0.1f)
                {
                    Debug.LogError("This is not what the bat is standing on!");
                }

                goToReset = tempHit.transform.gameObject;
                goToReset.layer = 0;
            }
        }

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);

        //Re-enable boxCollider after linecast
        boxCollider.enabled = true;

        if (standingOn && goToReset != null)
        {
            goToReset.layer = 8;
        }

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
        base.Hit(dmg);

        if (health <= 0)
        {
            // remove from grid
            GameManager.Instance.LevelManager.TileMap.RemoveBat(x, y);

            if (target != null) target.Targeted = false;

            GameManager.Instance.RemoveCreature(this);

            Destroy(this.gameObject);
        }
        else
        {
            StartCoroutine(HitVisual());
        }
    }
}
