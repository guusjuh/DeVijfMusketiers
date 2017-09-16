using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private int totalActionPoints = 3;       // total points
    private int currentActionPoints;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints;} }

    private int health = 100;
    public int Health { get { return health; } }

    public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
    public LayerMask blockingLayer;         //Layer on which collision will be checked.

    private BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
    private float inverseMoveTime;          //Used to make movement more efficient.

    private Damagable target;
    private int x, y;
    private List<Node> currentPath = null;

    private HighlightButton highlightBttn;

    public void StartTurn()
    {
        currentActionPoints = totalActionPoints;
    }

    // initialize this instance
    public void Initialize(int x, int y)
    {
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

        // select the first target to destory! mwoehahaha
        target = SelectTarget();
        currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        while (currentPath == null)
        {
            // there is no way the creature can get to that target right now.
            Debug.Log("Creature did not find a valid path");
            target.Targeted = false;

            target = SelectTarget();
            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
        }
    }

    // move returns true if it is able to move and false if not
    private bool Move(int xDir, int yDir, out RaycastHit2D hit)
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

    // co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    private IEnumerator SmoothMovement(Vector3 end)
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

    // checks for collisions e.d.
    private void AttemptMove() 
    {
        RaycastHit2D hit;

        // remove the node we are standing on
        currentPath.RemoveAt(0);

        // no steps to take anymore
        if (currentPath.Count > 1)
        {
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

            // check if nothing was hit by linecast
            if (hit.transform == null)
                // if nothing was hit, return and don't execute further code.
                return;
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
    }

    private void OnCantMove(Damagable other) {
        //Debug.Log("I attack!");
        other.Targeted = false;

        // hit the other
        other.Hit();

        currentActionPoints = 0;
    }

    // called by GM to attempt move / attack
    public void MoveEnemy()
    {   
        //Debug.Log("New turn");

        // attempt the move
        AttemptMove();

        // if the target is hit, it has been set to null
        if (target == null)
        {
            // attempt to select a new target
            target = SelectTarget();

            // game is lost, will go to game over from levelmanager
            if (target == null) return;

            currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            while (currentPath == null)
            {
                // there is no way the creature can get to that target right now.
                Debug.Log("Creature did not find a valid path");
                target.Targeted = false;

                target = SelectTarget();
                currentPath = GameManager.Instance.LevelManager.TileMap.GeneratePathTo(x, y, target.x, target.y);
            }

            //Debug.Log("new target selected "+ target.transform.position + " " + target.gameObject.name);
        }

        currentActionPoints--;
    }

    private Damagable SelectTarget()
    {
        // declare super ugly unoptimazed list and arrays
        List<GameObject> possibleTargets = new List<GameObject>();
        Damagable[] tempTargets = FindObjectsOfType(typeof(Damagable)) as Damagable[];

        // only add vases which are destroyed already
        for (int i = 0; i < tempTargets.Length; i++)
        {
            if (tempTargets[i].type == DamagableType.Vase)
            {
                if (tempTargets[i].GetComponent<Vase>().Destroyed)
                {
                    continue;
                }
            }

            if (tempTargets[i].type == DamagableType.Human)
            {
                if (tempTargets[i].GetComponent<Human>().dead)
                {
                    continue;
                }
            }

            possibleTargets.Add(tempTargets[i].gameObject);
        }

        if (possibleTargets.Count <= 0)
        {
            return null;
        }
        else
        {
            // select a target
            int selection = UnityEngine.Random.Range(0, possibleTargets.Count);
            possibleTargets[selection].GetComponent<Damagable>().Targeted = true;
            return possibleTargets[selection].GetComponent<Damagable>();
        }
    }

    public void Hit(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            Application.LoadLevel("Win");
        }
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
