public class BombController : EnemyBulletController
{
    protected override void SetParameters()
    {
        base.SetParameters();
        Attack = 50;
        CanBeDestroyed = true;
    }
}
