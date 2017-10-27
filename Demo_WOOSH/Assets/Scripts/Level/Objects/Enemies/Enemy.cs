using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Xml.Schema;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Enemy : WorldObject
{
    // spell effects
    protected bool slowed = false;
    protected int slowCount = 0; // for how many turns am I still slowed
    public bool Slowed { get { return slowed; } }
    private GameObject frozenIcon;

    protected bool burning = false;
    public bool Burning { get { return burning; } }
    protected int burnDamage;
    protected int burnModifier = 0;
    protected int burnCount = 0;
    private GameObject burnedIcon;
    
    protected int calculatedTotalAP = 0;        // used to temporarily remove or add action points (for example slowed)
    protected int totalActionPoints = 3;        // total points
    protected int currentActionPoints;          // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    protected bool hasSpecial = true;
    public bool HasSpecial { get { return hasSpecial; } }

    protected int specialCost = 3;
    protected int specialCooldown = 0;
    protected int totalSpecialCooldown = 3;
    public int SpecialCooldown { get { return specialCooldown; } }

    protected int viewDistance = 1;

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

    public Sprite SpellIconSprite;

    private bool selectedInUI = true;
    public bool SelectedInUI { get { return selectedInUI; } }

    public bool Dead { get; private set; }

    private bool canFly = false;
    public bool CanFly { get { return canFly; } }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        Dead = false;

        burnedIcon = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/StatusEffects/BurnedIcon"),
            Vector3.zero, Quaternion.identity, UIManager.Instance.InGameUI.AnchorCenter);
        burnedIcon.SetActive(false);
        burnedIcon.transform.SetAsFirstSibling();

        frozenIcon = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/StatusEffects/FrozenIcon"),
            Vector3.zero, Quaternion.identity, UIManager.Instance.InGameUI.AnchorCenter);
        frozenIcon.SetActive(false);
        frozenIcon.transform.SetAsFirstSibling();

        // obtain components
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // bit optimized cuz no division
        inverseMoveTime = 1f / moveTime;

        calculatedTotalAP = totalActionPoints;
        currentActionPoints = calculatedTotalAP;
        health = startHealth;

        possibleSpellTypes.Add(GameManager.SpellType.Attack);
        possibleSpellTypes.Add(GameManager.SpellType.FrostBite);
        possibleSpellTypes.Add(GameManager.SpellType.Fireball);
    }


    public override void Reset()
    {
        base.Reset();
        calculatedTotalAP = totalActionPoints;
        currentActionPoints = calculatedTotalAP;
        health = startHealth;
        specialCooldown = 0;

        slowCount = 0;
        slowed = false;
        burnCount = 0;
        burning = false;
        ShowStatusEffects();
    }

    public override void ResetToInitDEVMODE(Coordinate startPos)
    {
        base.ResetToInitDEVMODE(startPos);
        Dead = false;
    }

    public override void Clear()
    {
        DestroyStatusIcons();

        GameManager.Instance.LevelManager.RemoveObject(this);
    }

    protected virtual void Attack(EnemyTarget other)
    {
        // hit the other
        other.Hit();
    }

    public override bool TryHit(int dmg)
    {
        if (base.TryHit(dmg))
        {
            Hit(dmg);
            return true;
        }

        NewFloatingDmgNumber(0);

        return false;
    }

    protected virtual bool Hit(int dmg)
    {
        health -= dmg;
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

        if (health <= 0)
        {
            Dead = true;
            GameManager.Instance.TileManager.HidePossibleRoads();
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

            if (!UberManager.Instance.DevelopersMode)
            {
                DestroyStatusIcons();
            }
            else
            {
                burnedIcon.SetActive(false);
                frozenIcon.SetActive(false);
            }

            GameManager.Instance.LevelManager.RemoveObject(this);
            return true;
        }

        StartCoroutine(HitVisual());
        NewFloatingDmgNumber(dmg);

        return false;
    }

    protected IEnumerator HitVisual()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0, 0, 1);

        yield return new WaitForSeconds(0.35f);

        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);

        yield break;
    }

    private void NewFloatingDmgNumber(float dmg)
    {
        FloatingIndicator newFloatingIndicator = new FloatingIndicator();
        newFloatingIndicator.Initialize(dmg.ToString(), Color.red, 4.0f, 0.5f, transform.position);
    }

    protected virtual void Heal(int amount)
    {
        if(Health + amount > startHealth){ health = startHealth; }
        else{ health += amount; }
    }

    private void DestroyStatusIcons()
    {
        Destroy(burnedIcon);
        Destroy(frozenIcon);
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

    public virtual void ShowPossibleRoads()
    {
        GameManager.Instance.TileManager.HidePossibleRoads();
        GameManager.Instance.TileManager.ShowPossibleRoads(this, gridPosition, calculatedTotalAP);
    }

    public void Slow(int turns)
    {
        if (!slowed)
        {
            slowCount = turns;
            slowed = true;
            calculatedTotalAP--;
            currentActionPoints = calculatedTotalAP;
            ShowPossibleRoads();
        }
        else
        {
            slowCount += turns;
        }

        ShowStatusEffects();
        SetUIInfo();
    }

    public void Burn(int turns, int burnDamage)
    {
        this.burnDamage = burnDamage + burnModifier;
        if (burnCount > 0)
        {
            burnCount += turns;
        }
        else
        {
            burnCount = turns;
            burning = true;
        }

        ShowStatusEffects();
    }

    public void ShowStatusEffects()
    {
        if (Dead) return;

        if (burnCount > 0 && slowed)
        {
            burnedIcon.SetActive(true);
            frozenIcon.SetActive(true);
            Vector2 canvasPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.TileManager.GetWorldPosition(GridPosition));
            frozenIcon.GetComponent<RectTransform>().anchoredPosition = canvasPos - new Vector2(25.0f, 40.0f);

            canvasPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.TileManager.GetWorldPosition(GridPosition));
            burnedIcon.GetComponent<RectTransform>().anchoredPosition = canvasPos - new Vector2(-25.0f, 40.0f);
        }
        else if (burnCount > 0)
        {
            frozenIcon.SetActive(false);

            burnedIcon.SetActive(true);
            Vector2 canvasPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.TileManager.GetWorldPosition(GridPosition));
            burnedIcon.GetComponent<RectTransform>().anchoredPosition = canvasPos - new Vector2(0, 40.0f);
        }
        else if (slowed)
        {
            burnedIcon.SetActive(false);

            frozenIcon.SetActive(true);
            Vector2 canvasPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.TileManager.GetWorldPosition(GridPosition));
            frozenIcon.GetComponent<RectTransform>().anchoredPosition = canvasPos - new Vector2(0, 40.0f);
        }
        else
        {
            burnedIcon.SetActive(false);
            frozenIcon.SetActive(false);
        }
    }

    public virtual void StartTurn()
    {
        if (specialCooldown > 0)
        {
            specialCooldown--;
        }

        GameManager.Instance.CameraManager.LockTarget(this.transform);
        SetUIInfo();
    }

    public virtual void EndTurn()
    {
        HandleSlow();
        HandleBurn();

        currentActionPoints = calculatedTotalAP;

        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();
    }

    private void HandleSlow()
    {
        if (slowCount > 0)
        {
            slowCount--;
            if (slowCount == 0)
            {
                slowed = false;
                //TODO: account for possible other slow effects
                calculatedTotalAP = totalActionPoints;
            }
        }
    }

    private void HandleBurn()
    {
        if (burnCount > 0)
        {
            if (burning)
            {
                Hit(burnDamage);
            }

            burnCount--;
            if (burnCount == 0)
            {
                burning = false;
            }
        }
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
        GameManager.Instance.TileManager.MoveObject(gridPosition, gridPosition + direction, this);

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
        Dictionary<EnemyTarget, int> possibleTargets = new Dictionary<EnemyTarget, int>();
        List<EnemyTarget> damagables = new List<EnemyTarget>();

        damagables.AddRange(GameManager.Instance.LevelManager.Humans.Cast<EnemyTarget>());
        damagables.AddRange(GameManager.Instance.LevelManager.Shrines.Cast<EnemyTarget>());

        //add all possible targets to possible target list
        for (int i = 0; i < damagables.Count; i++)
        {
            // dont add targets that cannot be targeted, are invisible (human), or are inactive (shrine)
            if (!damagables[i].CanBeTargeted || (damagables[i].IsShrine()))           //shrines aren't damagable currently
                    continue;

            //calculate probability 
            // humans   = 100 * reputation
            int probability = (100 * damagables[i].GetComponent<Human>().ContractRef.Reputation);

            possibleTargets.Add(damagables[i], probability);
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
        // select a target
        else
        {
            // sum all probabilities 
            int summedProbability = 0;
            foreach (KeyValuePair<EnemyTarget, int> entry in possibleTargets) summedProbability += entry.Value;

            // perform a random roll to find a random humantype
            int counter = 0;
            int randomRoll = (int)UnityEngine.Random.Range(0, summedProbability);

            // find the human type matching the random roll
            foreach (KeyValuePair<EnemyTarget, int> entry in possibleTargets)
            {
                counter += entry.Value;
                if (randomRoll < counter)
                {
                    target = entry.Key;
                    prevTarget = target;
                    break;
                }
            }

            //generate path to chosen target
            currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, target.GridPosition, this);

            // if no path was found
            if (currentPath == null)
            {
                // is there still another possible target?
                if (possibleTargets.Count > 1)
                {
                    // generate and check path for each target
                    foreach (KeyValuePair<EnemyTarget, int> entry in possibleTargets)
                    {
                        currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, entry.Key.GridPosition, this);
                        
                        // if we found a valid path, return
                        if (currentPath != null)
                        {
                            target = entry.Key.GetComponent<EnemyTarget>();
                            prevTarget = target;
                            return;
                        }
                    }
                }
                // no other possible targets, skip turn
                else
                {
                    target = null;
                    prevTarget = null;
                    currentPath = null;
                    return;
                }
            }
        }
    }

    public EnemyTarget SelectTargetViewDist()
    {
        List<EnemyTarget> possibleTargets = new List<EnemyTarget>();
        List<EnemyTarget> damagables = new List<EnemyTarget>();

        damagables.AddRange(GameManager.Instance.LevelManager.Humans.Cast<EnemyTarget>());

        //add all possible targets to possible target list
        for (int i = 0; i < damagables.Count; i++)
        {
            // dont add targets that cannot be targeted, are invisible (human), or are inactive (shrine)
            if (!damagables[i].CanBeTargeted)
                continue;

            // dont add targets that arnt in view dist
            if(!GameManager.Instance.TileManager.InRange(viewDistance, this, damagables[i])) 
                continue;

            possibleTargets.Add(damagables[i]);
        }

        // no one in range
        if (possibleTargets.Count <= 0)
        {
            return null;
        }

        // select target in range
        // if our current target is in the possibilities, stay on him
        if (possibleTargets.Contains(target))
        {
            return target;
        }
        return possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];      
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
        // if, select one of them as my target
        // else, old implementation
        EnemyTarget targetNearby = SelectTargetViewDist();

        // if there was a target nearby, set it to the current target
        if (targetNearby != null) {
            target = targetNearby;
            prevTarget = target;
        }    

        // do I have a target at all?
        if (target == null)
            SelectTarget(); // select a new target
        else
        {
            // update the current path to the original OR nearby target
            currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition,
                            target.GridPosition,
                            this);

            // if there is no possible route to our target right now, select a new target. 
            // if there are no other possiblities, the enemy will skip a turn.
            if (currentPath == null) SelectTarget();
        }
    }

    public bool TargetNearby()
    {
        return false;
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

        ShowPossibleRoads();
    }

    public override bool IsMonster() { return true; }
}
