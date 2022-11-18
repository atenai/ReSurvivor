using UnityEngine;

public class GrenadeController : EnemyBulletController
{
    protected float RotationSpeed;

    protected override void SetParameters()
    {
        base.SetParameters();
        Attack = 40;
        CanBeDestroyed = true;
    }

    protected override void Awake()
    {
        base.Awake();
        RotationSpeed = 180;
    }

    protected override void Update()
    {
        base.Update();
        transform.rotation *= Quaternion.Euler(new Vector3(0.0f, 0.0f, RotationSpeed * Time.deltaTime));
    }
}
