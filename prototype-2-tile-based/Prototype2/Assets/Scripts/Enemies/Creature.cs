using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : BaseEnemy
{
    // initialize this instance
    public override void Initialize(int x, int y)
    {
        base.Initialize(x, y);

        totalActionPoints = 3;
        health = 100;

        // select the first target to destory! mwoehahaha
        target = SelectTarget();
        prevTarget = target;

        currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        while (currentPath == null)
        {
            // there is no way the creature can get to that target right now.
            Debug.Log("Creature did not find a valid path");
            target.Targeted = false;

            target = SelectTarget();
            prevTarget = target;
            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        }

        if (currentPath.Count <= 2)
        {
            target.Targeted = true;
        }
    }

    // move returns true if it is able to move and false if not
    //TODO: inheritance need-o-potato
    protected override bool Move(int xDir, int yDir, out RaycastHit2D hit)
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
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            StartCoroutine(SmoothMovement(end));

            //Return true to say that Move was successful
            return true;
        }

        //If something was hit, return false, Move was unsuccesful.
        return false;
    }

    // checks for collisions e.d.
    //TODO: inheritance need-o-potato
    protected override bool AttemptMove(bool secondTry = true) 
    {
        RaycastHit2D hit;

        // remove the node we are standing on
        if(secondTry) currentPath.RemoveAt(0);

        if (currentPath.Count <= 2)
        {
            target.Targeted = true;
        }

        // no steps to take anymore
        if (currentPath.Count > 1)
        {
            //TODO: check for barrel

            // set dir to the next 
            int xDir = currentPath[0].x - x;
            int yDir = currentPath[0].y - y;

            // set canMove to true if Move was successful, false if failed.
            // if it's false, we reached target, ready to attack
            bool canMove = Move(xDir, yDir, out hit);

            // if we could move, update the x and y pos
            if (canMove)
            {
                x += xDir;
                y += yDir;
            }
            else
            {
                if (hit.transform.GetComponent<Barrel>() != null)
                {
                    OnCantMove(hit.transform.GetComponent<Barrel>());
                    return true;
                }
            }

            // check if nothing was hit by linecast
            if (hit.transform == null)
                // if nothing was hit, return and don't execute further code.
                return false;
        }
        else
        {
            //if (hit.transform.gameObject.transform == target)
            //{
                OnCantMove(target);
            //}

            // set the target to null so the next turn, searching for new target
            target = null;
            currentPath = null;
        }

        return false;
    }

    protected override void OnCantMove(Damagable other) {
       base.OnCantMove(other);
    }

    // called by GM to attempt move / attack
    //TODO: inheritance need-o-potato
    public override bool MoveEnemy(bool secondTry = true)
    {   
        //Debug.Log("New turn");

        // attempt the move
        bool hitBarrel = AttemptMove(secondTry);

        // target cannot be null, not reached yet
        if(hitBarrel) { return false; }

        // if the target is hit, it has been set to null
        if (target == null)
        {
            // attempt to select a new target
            target = SelectTarget();
            prevTarget = target;

            // game is lost, will go to game over from levelmanager
            if (target == null) return true;

            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            while (currentPath == null)
            {
                // there is no way the creature can get to that target right now.
                Debug.Log("Creature did not find a valid path");
                target.Targeted = false;

                target = SelectTarget();
                prevTarget = target;
                currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            }

            if (currentPath.Count <= 2)
            {
                target.Targeted = true;
            }

            //Debug.Log("new target selected "+ target.transform.position + " " + target.gameObject.name);
        }

        currentActionPoints--;

        return true;
    }

    public override void Hit(int dmg)
    {
        base.Hit(dmg);

        if (health <= 0)
        {
            Application.LoadLevel("Win");
        }
        else
        {
            StartCoroutine(HitVisual());
        }
    }
}
