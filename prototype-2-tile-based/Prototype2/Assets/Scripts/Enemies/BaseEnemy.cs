using System.Collections;
using System.Collections.Generic;
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
    protected int x, y;
    protected List<Node> currentPath = null;

    protected HighlightButton highlightBttn;

    // sets the action points to all the actinpoints
    public void StartTurn() { currentActionPoints = totalActionPoints; }

    public virtual void Initialize(int x, int y) {
        this.x = x;
        this.y = y;

        highlightBttn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>();
        highlightBttn.transform.SetParent(FindObjectOfType<Canvas>().transform);
        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
        highlightBttn.Deactive();

        // obtain components
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        // bit optimized cuz no division
        inverseMoveTime = 1f / moveTime;
    }

    protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
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

        //If something was hit, return false, Move was unsuccesful.
        return false;
    }

    protected virtual bool AttemptMove(bool secondTry = true)
    {
        return true;
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

    public virtual bool MoveEnemy(bool secondTry = true)
    {
        return true;
    }

    //TODO: different chances and stuff
    protected Damagable SelectTarget()
    {
        // declare super ugly unoptimazed list and arrays
        List<GameObject> possibleTargets = new List<GameObject>();
        Damagable[] tempTargets = FindObjectsOfType(typeof(Damagable)) as Damagable[];

        // only add vases which are destroyed already
        for (int i = 0; i < tempTargets.Length; i++)
        {
            if (tempTargets[i].type == DamagableType.Barrel)
            {
                //if (tempTargets[i].GetComponent<Barrel>().Destroyed)
                //{
                    continue;
                //}
            }

            if (tempTargets[i].type == DamagableType.Human)
            {
                // dont target dead humans
                if (tempTargets[i].GetComponent<Human>().dead)
                {
                    continue;
                }
            }

            if (tempTargets[i].type == DamagableType.Shrine)
            {
                // dont target destory or nonactive shrines
                if (tempTargets[i].GetComponent<Shrine>().destroyed || !tempTargets[i].GetComponent<Shrine>().Active)
                {
                    continue;
                }
            }

            possibleTargets.Add(tempTargets[i].gameObject);
        }

        // first select target this is null
        if (prevTarget != null)
            possibleTargets.Remove(prevTarget.gameObject);

        if (possibleTargets.Count <= 0)
        {
            return prevTarget;
        }
        else
        {
            // select a target
            int selection = UnityEngine.Random.Range(0, possibleTargets.Count);
            //possibleTargets[selection].GetComponent<Damagable>().Targeted = true;
            return possibleTargets[selection].GetComponent<Damagable>();
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
            highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
            highlightBttn.Activate(bttn, this.gameObject);
        }
        else
        {
            highlightBttn.Deactive();
        }
    }
}
