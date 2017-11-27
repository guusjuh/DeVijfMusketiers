public class Dodin : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        startHealth = 100;
        totalActionPoints = 3;
        viewDistance = 4;
        type = SecContentType.Dodin;
        canFly = true;

        //add actions
        actions.Add(new EnemyFireBall());
        actions.Add(new EnemyMove());

        base.Initialize(startPos);
    }
}
