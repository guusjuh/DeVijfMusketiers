﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    // action points
    protected int totalActionPoints = 3;       // total points
    protected int currentActionPoints;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    protected int health = 100;
    public int Health { get { return health; } }

    public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    public LayerMask blockingLayer;         //Layer on which collision will be checked.

    protected BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
    protected Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    protected float inverseMoveTime;          //Used to make movement more efficient.

    protected Damagable target;
    protected Damagable prevTarget;
    public int x, y;
    protected List<Node> currentPath = null;

    protected HighlightButton highlightBttn;

    // sets the action points to all the actinpoints
    public void StartTurn() { currentActionPoints = totalActionPoints; }

    public virtual void Initialize(int x, int y) {
        this.x = x;
        this.y = y;

        InitializeHighlight();

        // obtain components
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // bit optimized cuz no division
        inverseMoveTime = 1f / moveTime;
    }

    private void InitializeHighlight()
    {
        highlightBttn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>();
        highlightBttn.transform.SetParent(FindObjectOfType<Canvas>().transform);
        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
        highlightBttn.Deactive();
    }
    
    // co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter.
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
    }

    protected virtual void OnCantMove(Damagable other)
    {
        //Debug.Log("I attack!");
        other.Targeted = false;

        // hit the other
        other.Hit();
    }

    protected virtual bool CanMove(int xDir, int yDir, out RaycastHit2D hit)
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

    public virtual void MoveEnemy()
    {
    }

    //TODO: different chances and stuff
    protected void SelectTarget()
    {
        if (target != null) target.Targeted = false;

        // declare super ugly unoptimazed list and arrays
        List<GameObject> possibleTargets = new List<GameObject>();
        Damagable[] tempTargets = FindObjectsOfType(typeof(Damagable)) as Damagable[];

        // only add vases which are destroyed already
        for (int i = 0; i < tempTargets.Length; i++)
        {
            // barrels are no target
            if (tempTargets[i].type == DamagableType.Barrel) continue;

            // dont target dead humans
            if (tempTargets[i].type == DamagableType.Human)
            {
                if (tempTargets[i].GetComponent<Human>().dead || tempTargets[i].GetComponent<Human>().Invisible) continue;
            }

            // dont target destoryed or nonactive shrines
            if (tempTargets[i].type == DamagableType.Shrine)
            {
                if (tempTargets[i].GetComponent<Shrine>().destroyed || !tempTargets[i].GetComponent<Shrine>().Active) continue; 
            }

            possibleTargets.Add(tempTargets[i].gameObject);
        }

        // first call to SelectTarget this is null
        if (prevTarget != null)
            possibleTargets.Remove(prevTarget.gameObject);

        // no possible targets were found
        if (possibleTargets.Count <= 0)
        {
            target = null;
            prevTarget = null;
        }
        else
        {
            // select a target
            int selection = UnityEngine.Random.Range(0, possibleTargets.Count);
            target = possibleTargets[selection].GetComponent<Damagable>();
            prevTarget = target;
        }
    }

    public virtual void Hit(int dmg)
    {
        health -= dmg;
    }

    protected IEnumerator HitVisual()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 0, 1);

        yield return new WaitForSeconds(0.35f);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        yield break;
    }

    public void SetHighlight(bool value, SpellButton bttn)
    {
        if (value)
        {
            highlightBttn.Activate(bttn, this.gameObject);
            highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(new Vector3(this.transform.position.x, this.transform.position.y, 0));
        }
        else
        {
            highlightBttn.Deactive();
        }
    }

    public void UpdateTarget()
    {
        // if no current path avaible (blocked last step)
        if (currentPath == null)
        {
            if (target == null)
            {
                // attempt to select a new target
                SelectTarget();
                
                // no possible targets have been found
                if (target == null)
                {
                    currentActionPoints = 0;
                    return;
                }
            }

            // is my target a human? than i have to check if he's not invisible
            if (target.type == DamagableType.Human && target.GetComponent<Human>().Invisible)
            {
                SelectTarget();

                // no possible targets have been found
                if (target == null)
                {
                    currentActionPoints = 0;
                    return;
                }

                currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            }

            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            if (currentPath == null)
            {
                currentActionPoints = 0;
                return;
            }
        }

        // do I still have a target?
        if (target != null)
        {
            // is my target a human? than i have to check if he's not invisible
            if (target.type == DamagableType.Human && target.GetComponent<Human>().Invisible)
            {
                SelectTarget();

                // no possible targets have been found
                if (target == null)
                {
                    currentActionPoints = 0;
                    return;
                }

                currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            }
            else
            {
                currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);

                if (currentPath == null)
                {
                    currentActionPoints = 0;
                    return;
                }
            }
        }
        // target is dead/broken
        else
        {
            // attempt to select a new target
            SelectTarget();

            // game is lost, will go to game over from levelmanager
            if (target == null)
            {
                currentActionPoints = 0;
                return;
            }

            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);

            if (currentPath == null)
            {
                currentActionPoints = 0;
                return;
            }

        }

        if (currentPath.Count <= 2) target.Targeted = true;
    }
}
