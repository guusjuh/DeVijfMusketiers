using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : WorldObject
{
    public const string MOVE_ANIM = "Moving";
    public const string HIT_ANIM = "Hurt";
    public const string ATTACK_ANIM = "Attack";
    public const string DIE_ANIM = "Dead";

    // spell effects
    private bool slowed = false;
    private int slowCount = 0; // for how many turns am I still slowed
    public bool Slowed { get { return slowed; } }
    private GameObject frozenIcon;

    private bool burning = false;
    public bool Burning { get { return burning; } }
    private int burnDamage;
    private int burnModifier = 0;
    private int burnCount = 0;
    private GameObject burnedIcon;

    private const float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    private float inverseMoveTime;          //Used to make movement more efficient.
    public float InverseMoveTime { get { return inverseMoveTime; } }

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    private List<SpriteRenderer> sprRenders;
    public List<SpriteRenderer> SprRenders { get { return sprRenders; } }

    private int calculatedTotalAP = 0;        // used to temporarily remove or add action points (for example slowed)
    protected int totalActionPoints = 3;        // total points
    private int currentActionPoints;          // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    protected int viewDistance = 1;

    protected float startHealth = 10;
    public float StartHealth { get { return startHealth; } }
    private float health;
    public float Health { get { return health; } }
    public float HealthPercentage { get { return (health / startHealth) * 100; } }

    protected EnemyTarget target;
    public EnemyTarget Target { get { return target; } }
    public void NoPossibleTarget() { target = null; } 

    private EnemyTarget prevTarget;
    
    private bool selectedInUI = true;
    public bool SelectedInUI { get { return selectedInUI; } }

    public bool Dead { get; private set; }

    protected bool canFly = false;
    public bool CanFly { get { return canFly; } }

    protected List<Action> actions = new List<Action>();
    public List<Action> Actions { get { return actions; } }

    public override void Initialize(Coordinate startPos)
    {
        base.Initialize(startPos);

        anim = gameObject.GetComponentInChildren<Animator>();
        sprRenders = new List<SpriteRenderer>(gameObject.GetComponentsInChildren<SpriteRenderer>());

        Dead = false;

        burnedIcon = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGame/StatusEffects/BurnedIcon"),
            Vector3.zero, Quaternion.identity, UIManager.Instance.InGameUI.AnchorCenter);
        burnedIcon.SetActive(false);
        burnedIcon.transform.SetAsFirstSibling();

        frozenIcon = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/InGame/StatusEffects/FrozenIcon"),
            Vector3.zero, Quaternion.identity, UIManager.Instance.InGameUI.AnchorCenter);
        frozenIcon.SetActive(false);
        frozenIcon.transform.SetAsFirstSibling();

        // bit optimized cuz no division
        inverseMoveTime = 1f / moveTime;

        calculatedTotalAP = totalActionPoints;
        currentActionPoints = calculatedTotalAP;
        health = startHealth;

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Initialize(this);
        }

        if (UberManager.Instance.Tutorial)
        {
            possibleSpellTypes.Add(GameManager.SpellType.Attack);
            return;
        }

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

        slowCount = 0;
        slowed = false;
        burnCount = 0;
        burning = false;
        ShowStatusEffects();
        transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Reset();
        }
    }

    public override void ResetToInitDEVMODE(Coordinate startPos)
    {
        base.ResetToInitDEVMODE(startPos);
        Dead = false;
        anim.SetBool(DIE_ANIM, false);
        sprRenders.HandleAction(s => s.color = new Color(1, 1, 1, 1));
    }

    public override void Clear()
    {
        DestroyStatusIcons();

        GameManager.Instance.LevelManager.RemoveObject(this);
    }

    public void Attack(EnemyTarget other)
    {
        // hit the other
        other.Hit();
        anim.SetTrigger(ATTACK_ANIM);
    }

    public void UpdateGridPosition(Coordinate direction)
    {
        gridPosition += direction;
    }

    public override bool TryHit(int dmg)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (!actions[i].TryHit())
            {
                NewFloatingDmgNumber(0);
                return false;
            }
        }

        if (base.TryHit(dmg))
        {
            Hit(dmg);
            return true;
        }

        NewFloatingDmgNumber(0);

        return false;
    }

    public void Kill()
    {
        Hit((int)Health);
    }

    protected override bool Hit(int dmg)
    {
        health -= dmg;
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

        StartCoroutine(HitVisual());
        NewFloatingDmgNumber(dmg);

        if (health <= 0)
        {
            GooglePlayScript.UnlockAchievement(GooglePlayIds.achievement_first_blood);

            Dead = true;
            anim.SetBool(DIE_ANIM, true);
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

        return false;
    }

    private IEnumerator HitVisual()
    {
        anim.SetTrigger(HIT_ANIM);

        sprRenders.HandleAction(s => s.color = new Color(0.8f, 0, 0, 1));

        Instantiate(Resources.Load<GameObject>("Prefabs/HitParticle"), transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.35f);

        sprRenders.HandleAction(s => s.color = new Color(1, 1, 1, 1));
    }

    private void NewFloatingDmgNumber(float dmg)
    {
        FloatingIndicator newFloatingIndicator = new FloatingIndicator();
        newFloatingIndicator.Initialize(dmg.ToString(), Color.red, 4.0f, 0.5f, transform.position);
    }

    public void Heal(int amount)
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

    public void ShowPossibleRoads()
    {
        GameManager.Instance.TileManager.HidePossibleRoads();
        GameManager.Instance.TileManager.ShowPossibleRoads(this, gridPosition, calculatedTotalAP);

        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].ShowPossibleRoads();
        }
    }

    public void Slow(int turns)
    {
        if (!slowed)
        {
            slowCount = turns;
            slowed = true;
            calculatedTotalAP--;
            currentActionPoints = calculatedTotalAP;
            if(GameManager.Instance.LevelManager.PlayersTurn) ShowPossibleRoads();
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

    public void StartTurn()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].StartTurn();
        }

        GameManager.Instance.CameraManager.LockTarget(this.transform);
        SetUIInfo();
    }

    public void EndTurn()
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

    public void EnemyMove()
    {
        if(!CheckTarget()) return;

        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].DoAction()) return;
        }
        
        EndMove(1);
    }

    private void PathBlocked(Transform other)
    {
        if (other.GetComponent<Rock>() != null) Attack(other.GetComponent<Rock>());
    }

    public void EndMove(int cost)
    {
        // lose one action point
        if (currentActionPoints > 0)
        {
            currentActionPoints -= cost;
        }

        SetUIInfo();
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
            List<TileNode> currentPath = GameManager.Instance.TileManager.GeneratePathTo(gridPosition, target.GridPosition, this);

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
        if (target == null || prevTarget == null)
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
    }

    public bool TargetNearby()
    {
        return false;
    }

    private void SetUIInfo()
    {
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(currentActionPoints <= 0 ? null : this);
    }

    public void TargetReached()
    {
        Attack(target);

        // set the target to null so the next turn, searching for new target
        target = null;
    }

    public override void Click()
    {
        base.Click();

        if (!GameManager.Instance.LevelManager.PlayersTurn) return;
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

        ShowPossibleRoads();
    }

    public override bool IsMonster() { return true; }

    public override bool IsWalking()
    {
        return !canFly;
    }

    public override bool IsFlying()
    {
        return canFly;
    }
}
