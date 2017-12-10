public class Ogre : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        startHealth = 100;
        totalActionPoints = 3;
        viewDistance = 4;
        type = SecContentType.Ogre;
        canFly = false;

        //add actions
        actions.Add(new EnemyBrokenGround());
        actions.Add(new EnemyMove());

        base.Initialize(startPos);
    }
}
