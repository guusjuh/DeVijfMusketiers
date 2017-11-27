public class Arnest : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        startHealth = 100;
        totalActionPoints = 3;
        viewDistance = 3;
        type = SecContentType.Arnest;
        canFly = false;

        //add actions
        actions.Add(new EnemyHeal());
        actions.Add(new EnemyMove());

        base.Initialize(startPos);
    }
}
