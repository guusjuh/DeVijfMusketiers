public class Wolf : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        if (UberManager.Instance.Tutorial)
            startHealth = 10;
        else
            startHealth = 30;
        totalActionPoints = 2;        
        viewDistance = 3;
        type = SecContentType.Wolf;
        canFly = false;

        //add actions
        actions.Add(new EnemyMove());

        base.Initialize(startPos);
    }
}
