using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Human : EnemyTarget
{
    protected const string MOVE_ANIM = "Moving";
    protected const string CHEER_ANIM = "Cheer";
    protected const string DIE_ANIM = "Dead";
    protected const string PANIC_ANIM = "Panic";

    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    private const float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    private float inverseMoveTime;          //Used to make movement more efficient.

    private GameObject childGO;
    private Animator anim;
    private List<SpriteRenderer> sprRenders;
    public List<SpriteRenderer> SprRenders { get { return sprRenders; } }

    private int totalFleePoints = 2;
    private int currentFleePoints;
    public int CurrentFleePoints { get {return currentFleePoints; } }

    private int viewDistance = 4;

    private bool inPanic;
    public bool InPanic
    {
        get
        {
            CheckInPanic();
            return inPanic;
        }
    }

    private Contract contractRef;
    public Contract ContractRef
    {
        get { return contractRef; }
        set
        {
            contractRef = value;

            if(childGO != null) Destroy(childGO);
            childGO = Instantiate(contractRef.InWorld, Vector3.zero, Quaternion.identity, this.transform);
            childGO.transform.localPosition = new Vector2(0.0f, -0.4f);

            anim = gameObject.GetComponentInChildren<Animator>();
            sprRenders = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        }
    }

    public override void Initialize(Coordinate startPos)
    {
        spellTargetType = SpellManager.SpellTarget.Human;
        base.Initialize(startPos);

        type = SecContentType.Human;

        inverseMoveTime = 1f / moveTime;
        rb2D = GetComponent<Rigidbody2D>();

        currentFleePoints = totalFleePoints;
    }

    public override void Reset()
    {
        base.Reset();
        currentFleePoints = totalFleePoints;
        inPanic = false;
        anim.SetBool(PANIC_ANIM, false);
    }

    public override void ResetToInitDEVMODE(Coordinate startPos)
    {
        base.ResetToInitDEVMODE(startPos);
        anim.SetBool(DIE_ANIM, false);
        sprRenders.HandleAction(s => s.color = new Color(1, 1, 1, 1));
    }

    public void Cheer()
    {
        anim.SetTrigger(CHEER_ANIM);
    }

    public void StartTurn()
    {
        CheckInPanic();
        currentFleePoints = totalFleePoints;
    }

    private void CheckInPanic()
    {
        // find all enemies in the level
        List<Enemy> enemies = GameManager.Instance.LevelManager.Enemies;

        // check for each being in range
        for (int i = 0; i < enemies.Count; i++)
        {
            if (GameManager.Instance.TileManager.InRange(viewDistance, this, enemies[i]))
            {
                inPanic = true;
                anim.SetBool(PANIC_ANIM, true);
                return;
            }
        }

        inPanic = false;
        anim.SetBool(PANIC_ANIM, false);
    }

    public IEnumerator Flee()
    {
        currentFleePoints--;

        List<TileNode> neighbours = GameManager.Instance.TileManager.GetNodeReference(gridPosition).NeightBours;
        List<TileNode> fleeNodes = new List<TileNode>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            // 1. if the neighbour is occupied or not an enterable tile, continue
            // also continue if its a corner tile
            if (!neighbours[i].CompletelyEmpty() || 
                GameManager.Instance.TileManager.Corners.Contains(neighbours[i].GridPosition))
                    goto NOT_THIS_NEIGHBOUR;

            // 2. not towards an enemy
            // find all enemies in view distance
            List<Enemy> closeEnemies = GameManager.Instance.LevelManager.Enemies.FindAll(
                e => GameManager.Instance.TileManager.InRange(viewDistance, this, e));

            // if the distance between my tile and the enemy is greater than the distance betweent the neighbouring tile and the enemy
            // the hooman will be getting closer, so don't go to that tile
            float currentDist = 0;
            float neighbourDist = 0;
            for (int j = 0; j < closeEnemies.Count; j++)
            {
                currentDist = gridPosition.EuclideanDistance(closeEnemies[j].GridPosition);
                neighbourDist = neighbours[i].GridPosition.EuclideanDistance(closeEnemies[j].GridPosition);

                if (neighbourDist < currentDist)
                {
                    goto NOT_THIS_NEIGHBOUR;
                }
            }

            // 3. not directly next to a hole
/*            // find all holes in view distance
            List<TileNode> closeHoles = GameManager.Instance.TileManager.GetNodeWithGapReferences().FindAll(
                n => GameManager.Instance.TileManager.InRange((int)Mathf.Floor(viewDistance/2.0f), this.GridPosition, n.GridPosition));

            // check for the distance being smaller (just like with enemies)
            for (int j = 0; j < closeHoles.Count; j++)
            {
                currentDist = gridPosition.EuclideanDistance(closeHoles[j].GridPosition);
                neighbourDist = neighbours[i].GridPosition.EuclideanDistance(closeHoles[j].GridPosition);

                if (neighbourDist < currentDist)
                {
                    goto NOT_THIS_NEIGHBOUR;
                }
            }*/

            // if we get to this point, add to the possible flee tiles
            fleeNodes.Add(neighbours[i]);

            NOT_THIS_NEIGHBOUR: continue;
        }

        // if there is just no better tile to run to, just stay here
        if(fleeNodes.Count <= 0) yield break;

        //pick a random flee node 
        TileNode chosenNode = fleeNodes[UnityEngine.Random.Range(0, fleeNodes.Count)];

        //move to the chosen node
        GameManager.Instance.TileManager.MoveObject(gridPosition, chosenNode.GridPosition, this);
        gridPosition = chosenNode.GridPosition;

        yield return StartCoroutine(SmoothMovement(GameManager.Instance.TileManager.GetWorldPosition(chosenNode.GridPosition)));

        yield return null;
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

    public override void Clear()
    {
        GameManager.Instance.LevelManager.RemoveObject(this);
    }

    public override bool Hit()
    {
        canBeTargeted = false;

        Instantiate(Resources.Load<GameObject>("Prefabs/HitParticle"), transform.position, Quaternion.identity);

        anim.SetBool(DIE_ANIM, true);

        contractRef.Die();

        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.DyingHuman);

        GameManager.Instance.LevelManager.RemoveObject(this);

        return true;
    }

    public override void DeadByGap()
    {
        SoundManager.PlaySoundEffect(SoundManager.SoundEffect.DyingHuman);
        contractRef.Die();
        base.DeadByGap();
    }

    public override bool IsHuman() { return true; }
    public override bool IsWalking() { return true; }
}
