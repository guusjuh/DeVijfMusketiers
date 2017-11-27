public class Sketta : Enemy {
    public override void Initialize(Coordinate startPos)
    {
        startHealth = 130;
        totalActionPoints = 3;
        viewDistance = 3;
        type = SecContentType.Sketta;
        canFly = false;

        //add actions
        actions.Add(new EnemyBlock());
        actions.Add(new EnemyMove());

        base.Initialize(startPos);
    }
}
