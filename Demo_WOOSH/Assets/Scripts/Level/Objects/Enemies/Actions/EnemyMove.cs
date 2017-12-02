using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : Action
{
    protected LayerMask blockingLayer;         //Layer on which collision will be checked.
    private BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
    private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.

    public List<TileNode> currentPath = null;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        //set cost
        cost = 1;

        // obtain components
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        boxCollider = parent.GetComponent<BoxCollider2D>();
        rb2D = parent.GetComponent<Rigidbody2D>();

    }

    public override void StartTurn()
    {
        findPath();
    }

    private void findPath()
    {
        if (parent.target == null)
        {
            parent.SelectTarget();
        }

        //generate path to chosen target
        currentPath = GameManager.Instance.TileManager.GeneratePathTo(parent.GridPosition, parent.target.GridPosition, parent);

        // if no path was found
        if (currentPath == null)
        {
            // no other possible targets, skip turn
            parent.target = null;
        }
    }

    public override bool DoAction()
    {
        if(currentPath == null || parent.target == null || currentPath.Count == 0 )
        {
            return false;
        }

        // do raycast to check for world objects
        RaycastHit2D hit;

        Coordinate direction = currentPath[1].GridPosition - parent.GridPosition;

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

        parent.EndMove(cost);
        return true;
    }

    private void CantMove(Transform other)
    {
        // target reached
        if (currentPath.Count <= 2)
        {
            if (other.gameObject.transform == parent.target.transform)
            {
                parent.TargetReached();

                findPath();
            }
        }
        // barrel in my way
        else
            parent.Attack(other.GetComponent<EnemyTarget>());
    }

    // called walk, since 'move' is the complete turn of an enemy
    protected virtual void Walk(Coordinate direction)
    {
        // update pos in tile map
        GameManager.Instance.TileManager.MoveObject(parent.GridPosition, parent.GridPosition + direction, parent);

        // update x and y
        parent.UpdateGridPosition(direction);

        // perform move
        UberManager.Instance.StartCoroutine(SmoothMovement(GameManager.Instance.TileManager.GetWorldPosition(parent.GridPosition)));

        // remove current path[0], e.g. node i was standing on
        currentPath.RemoveAt(0);
    }


    // note: works for everything nonflying!
    protected virtual bool CanMove(Coordinate direction, out RaycastHit2D hit)
    {
        //Store start position to move from, based on objects current transform position.
        Coordinate start = parent.GridPosition;

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
        parent.anim.SetBool(Enemy.MOVE_ANIM, true);

        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter.
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (parent.transform.position - end).sqrMagnitude;

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, parent.InverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            rb2D.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (parent.transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }

        parent.anim.SetBool(Enemy.MOVE_ANIM, false);
    }
}
