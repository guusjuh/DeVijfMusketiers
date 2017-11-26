using System.Collections;
using UnityEngine;
public class EnemyFireBall : Action
{
    private Sprite spellIconSprite;
    private GameObject fireBall;
    private const int specialMaxDistance = 3;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        //disables the fireball
        fireBall = parent.transform.Find("FireBall").gameObject;
        fireBall.SetActive(false);

        spellIconSprite = Resources.Load<Sprite>("Sprites/UI/InGame/Spells/enemyFire");
    }

    public override void Reset()
    {
        base.Reset();
        fireBall.SetActive(false);
    }

    public override bool DoAction()
    {
        return base.DoAction();


        float distance = (parent.GridPosition.EuclideanDistance(parent.target.GridPosition));
        float maxDistance = specialMaxDistance * GameManager.Instance.TileManager.FromTileToTile;
        bool closeEnough = distance - maxDistance <= 0.01f;
        bool enoughAP = parent.CurrentActionPoints >= cost;
        bool onCooldown = currentCooldown > 0;

        if (closeEnough && enoughAP && !onCooldown)
        {
            parent.EndMove(cost);
            currentCooldown = totalCooldown;
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(parent);

            fireBall.transform.localPosition = Vector3.zero;

            UberManager.Instance.StartCoroutine(ShootFireBall(GameManager.Instance.TileManager.GetWorldPosition(parent.target.GridPosition)));
            return true;
        }
        return false;
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

        //While that distance is greater than a very small amount (Epsilon, almost zero):
        while (sqrRemainingDistance > 0.0001f)
        {
            //Find a new position proportionally closer to the end, based on the moveTime
            Vector3 newPostion = Vector3.MoveTowards(ball.position, end, parent.InverseMoveTime * Time.deltaTime);

            //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
            ball.MovePosition(newPostion);

            //Recalculate the remaining distance after moving.
            sqrRemainingDistance = (ball.transform.position - end).sqrMagnitude;

            //Return and loop until sqrRemainingDistance is close enough to zero to end the function
            yield return null;
        }
        fireBall.SetActive(false);
        parent.TargetReached();
    }

}
