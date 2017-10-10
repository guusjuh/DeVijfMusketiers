using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodin : Enemy
{
    private int freeAPCooldownTotal = 3;
    private int freeAPCooldown = 0;

    public GameObject FireBall;

    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 100;

        //diables the fireball
        FireBall.SetActive(false);

        freeAPCooldown = freeAPCooldownTotal;

        base.Initialize(startPos);
    }

    public override bool CheckForSpell()
    {
        // target reached
        if (currentPath.Count <= 3 && currentActionPoints >= 3)
        {
            currentActionPoints -= 3;
            FireBall.transform.localPosition = Vector3.zero;

            StartCoroutine(ShootFireBall(GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition)));
            return true;
        }
        return false;
    }

    public override void EnemyMove()
    {
        if (!CheckTarget()) return;

        if (CheckForSpell()) return;

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
            //check if free movement is off cooldown
            if (freeAPCooldown <= 0)
            {
                //gives an ap for free move
                currentActionPoints++;

                freeAPCooldown = freeAPCooldownTotal;
            }
            Walk(direction);
        }

        EndMove();
    }

    // co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator ShootFireBall(Vector3 end)
    {
        FireBall.SetActive(true);
        Rigidbody2D ball = FireBall.GetComponent<Rigidbody2D>();

        //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter.
        //Square magnitude is used instead of magnitude because it's computationally cheaper.
        float sqrRemainingDistance = (ball.transform.position - end).sqrMagnitude;
        //TODO: remove rounding error now resolved with a dirty fix.
        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > 0.0001f)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(ball.position, end, inverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            ball.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (ball.transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        FireBall.SetActive(false);
        TargetReached();
    }

    public override void EndTurn()
    {
        freeAPCooldown--;

        base.EndTurn();
    }
}
