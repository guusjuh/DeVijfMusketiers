using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodin : Enemy
{
    private GameObject fireBall;

    //TODO: refactor while improveming enemies
    // not all specials are actually an attack, some are like spawns
    // so not all have this distance and 
    // not all have to show extra tiles while showing highlighted tiles
    // goodluck with your SOR goestav :P
    private const int specialMaxDistance = 3;

    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 100;

        //disables the fireball
        fireBall = transform.Find("FireBall").gameObject;
        fireBall.SetActive(false);
        viewDistance = 4;

        this.hasSpecial = true;

        base.Initialize(startPos);

        type = TileManager.ContentType.FlyingMonster;
    }

    public override bool CheckForSpell()
    {
        // target reached
        float distance = (this.gridPosition.EuclideanDistance(target.GridPosition));
        float maxDistance = specialMaxDistance * GameManager.Instance.TileManager.FromTileToTile;// + (GameManager.Instance.TileManager.HexagonHeight * (Mathf.Abs(gridPosition.x % 2))); ;
        bool closeEnough = distance - maxDistance <= 0.01f;
        bool enoughAP = currentActionPoints >= specialCost;
        bool onCooldown = specialCooldown > 0;

        if (closeEnough && enoughAP && !onCooldown)
        {
            currentActionPoints -= specialCost;
            specialCooldown = totalSpecialCooldown;
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);

            fireBall.transform.localPosition = Vector3.zero;
            
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

            Walk(direction);
        }

        EndMove();
    }

    //TODO: why isn't there a fireball script??
    // co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
    protected IEnumerator ShootFireBall(Vector3 end)
    {
        fireBall.SetActive(true);
        Rigidbody2D ball = fireBall.GetComponent<Rigidbody2D>();

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
        fireBall.SetActive(false);
        TargetReached();
    }

    public override void ShowPossibleRoads()
    {
        base.ShowPossibleRoads();

        if (specialCooldown <= 0 && calculatedTotalAP >= specialCost) GameManager.Instance.TileManager.ShowExtraTargetForSpecial(gridPosition, specialMaxDistance);
    }
}
