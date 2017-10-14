using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public class Enemy : WorldObject
{
    protected int totalActionPoints = 3;       // total points
    protected int currentActionPoints;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    protected bool hasSpecial = true;
    public bool HasSpecial { get { return hasSpecial; } }

    protected int specialCost = 3;
    protected int specialCooldown = 0;
    public int SpecialCooldown { get { return specialCooldown; } }
    protected int totalSpecialCooldown = 3;

    protected float startHealth = 10;
    public float StartHealth { get { return startHealth; } }
    protected float health;
    public float Health { get { return health; } }
    public float HealthPercentage { get { return (health / startHealth) * 100; } }

    protected const float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    protected float inverseMoveTime;          //Used to make movement more efficient.

    protected LayerMask blockingLayer;         //Layer on which collision will be checked.
    protected BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
    protected Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.

    protected EnemyTarget target;
    protected EnemyTarget prevTarget;
    protected List<TileNode> currentPath = null;

    private bool selectedInUI = true;
    public bool SelectedInUI { get { return selectedInUI; } }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        // obtain components
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // bit optimized cuz no division
        inverseMoveTime = 1f / moveTime;

        currentActionPoints = totalActionPoints;
        health = startHealth;

        possibleSpellTypes.Add(GameManager.SpellType.Attack);

        type = TileManager.ContentType.WalkingMonster;

        //SetUIInfo();
    }

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveEnemy(this);
    }

    protected virtual void Attack(EnemyTarget other)
    {
        // hit the other
        other.Hit();
    }

    public virtual bool Hit(int dmg)
    {
        health -= dmg;
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

        if (health <= 0)
        {
            GameManager.Instance.TileManager.HidePossibleRoads();
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();
            GameManager.Instance.LevelManager.RemoveEnemy(this, true);
            return true;
        }

        return false;
    }

    protected IEnumerator HitVisual()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 0, 1);

        yield return new WaitForSeconds(0.35f);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        yield break;
    }

    public bool CheckTarget()
    {
        if (target == null)
        {
            SelectTarget();
        }
        if (!CheckTargetForSuperSafe()) return false;
        UpdateTarget();
        if (!CheckTargetForSuperSafe()) return false;

        return true;
    }

    public virtual bool CheckForSpell()
    {
        return false;
    }

    public virtual void StartTurn()
    {
        if (specialCooldown > 0)
        {
            specialCooldown--;
        }

        SetUIInfo();
    }

    public virtual void EndTurn()
    {
        currentActionPoints = totalActionPoints;
    }

    public virtual void EnemyMove()
    {
        if (CheckForSpell()) return;

        if(!CheckTarget()) return;

        // do raycast to check for world objects
        RaycastHit2D hit;

        Coordinate direction = currentPath[1].GridPosition - gridPosition;

        // can I move? 
        bool canMove = CanMove(direction, out hit);

        // either barrel on my way or target reached
        if (!canMove)
        {
            CantMove(hit.transform);
        }
        // else: move
        else
        {
            Walk(direction);
        }

        EndMove();
    }

    protected virtual void CantMove(Transform other)
    {   
        // target reached
        if (currentPath.Count <= 2)
        {
            if (other.gameObject.transform == target.transform) TargetReached();
        }
        // barrel in my way
        else
            Attack(other.GetComponent<EnemyTarget>());
    }

    protected virtual void TargetReached()
    {
         Attack(target);

        // set the target to null so the next turn, searching for new target
        target = null;
    }

    protected virtual void PathBlocked(Transform other)
    {
        if (other.GetComponent<Barrel>() != null) Attack(other.GetComponent<Barrel>());
    }

    // called walk, since 'move' is the complete turn of an enemy
    protected virtual void Walk(Coordinate direction)
    {
        // update pos in tile map
        GameManager.Instance.TileManager.MoveObject(gridPosition, gridPosition + direction, type);

        // update x and y
        gridPosition += direction;

        // perform move
        StartCoroutine(SmoothMovement(GameManager.Instance.TileManager.GetWorldPosition(gridPosition)));

        // remove current path[0], e.g. node i was standing on
        currentPath.RemoveAt(0);
    }

    protected virtual void EndMove()
    {
        // lose one action point
        if (currentActionPoints > 0)
        {
            currentActionPoints--;
        }

        SetUIInfo();
    }

    // note: works for everything nonflying!
    protected virtual bool CanMove(Coordinate direction, out RaycastHit2D hit)
    {
        //Store start position to move from, based on objects current transform position.
        Coordinate start = gridPosition;

        // Calculate end position based on the direction parameters passed in when calling Move.
        Coordinate end = start + direction;

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(GameManager.Instance.TileManager.GetWorldPosition(start), 
            GameManager.Instance.TileManager.GetWorldPosition(end), blockingLayer);

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

    public void SelectTarget()
    {
        // no target find a new one
        // dont get barrels!
        List<EnemyTarget> possibleTargets = new List<EnemyTarget>();
        List<EnemyTarget> damagables = new List<EnemyTarget>();

        damagables.AddRange(GameManager.Instance.LevelManager.Humans.Cast<EnemyTarget>());
        damagables.AddRange(GameManager.Instance.LevelManager.Shrines.Cast<EnemyTarget>());

        //add all possible targets to possible target list
        for (int i = 0; i < damagables.Count; i++)
        {
            // dont add targets that cannot be targeted, are invisible (human), or are inactive (shrine)
            if (!damagables[i].CanBeTargeted ||
                (damagables[i].Type == TileManager.ContentType.InivisbleHuman ||
                (damagables[i].Type == TileManager.ContentType.Shrine && !damagables[i].GetComponent<Shrine>().Active)))
                    continue;

            possibleTargets.Add(damagables[i]);
        }

        // first call to SelectTarget prevTarget is null
        // remove prevTarget to find a new one
        if (prevTarget != null && possibleTargets.Count > 1)
            possibleTargets.Remove(prevTarget);

        // no possible targets were found
        if (possibleTargets.Count <= 0)
        {
            Debug.Log("No possible targets");
            target = null;
            prevTarget = null;
            currentPath = null;
        }
        else
        {
            // select a target
            int selection = UnityEngine.Random.Range(0, possibleTargets.Count);
            target = possibleTargets[selection];
            prevTarget = target;

            //generate path to chosen target
            currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, target.GridPosition, type);

            // if no path was found
            if (currentPath == null)
            {
                // is there still another possible target?
                if (possibleTargets.Count > 1)
                {
                    // generate and check path for each target
                    for (int i = 0; i < possibleTargets.Count; i++)
                    {
                        currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, possibleTargets[i].GridPosition, type);
                        
                        // if we found a valid path, return
                        if (currentPath != null)
                        {
                            target = possibleTargets[i].GetComponent<EnemyTarget>();
                            prevTarget = target;
                            return;
                        }
                    }
                }
                // no other possible targets, skip turn
                else
                {
                    Debug.Log("No routes to targets");
                    target = null;
                    prevTarget = null;
                    currentPath = null;
                    return;
                }
            }
        }
    }

    public bool CheckTargetForSuperSafe()
    {
        if (currentPath == null || target == null || prevTarget == null)
        {/*
            if (currentPath == null && target == null && prevTarget == null)
            {*/
                currentActionPoints = 0;
                Debug.Log("skipped a move: no target or route found");
                return false;
            /*}
            Debug.LogError("Wubba lubba dup dup");
            return false;*/
        }
        return true;
    }

    public void UpdateTarget()
    {
        //TODO: somehow the target can be zero at this point, but it really shouldn't! find out how!
        
        // is my target a human? than i have to check if he's not invisible
        if (target == null || target.Type == TileManager.ContentType.InivisbleHuman)
        {
            SelectTarget();
        }
        else
        {
            currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, target.GridPosition, type);
        }
    }

    protected void SetUIInfo()
    {
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(currentActionPoints <= 0 ? null : this);
    }

    public override void Click()
    {
        base.Click();

        if (!GameManager.Instance.LevelManager.PlayersTurn) return;
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);
        GameManager.Instance.TileManager.ShowPossibleRoads(gridPosition, totalActionPoints);
    }
}
