using Model;
using UnityEngine;

public class TankController : PatrollerController
{
    protected override void SetParameters()
    {
        Hp = 7;
        RelativePosition = new Vector3(-0.22f, 0.01f, 0.0f);
        MAXSpeed = 0.5f;
        MoveForce = 20.0f;
    }
}
