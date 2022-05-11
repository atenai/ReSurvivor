using Model;
using UnityEngine;

public class TechnicalController : PatrollerController
{
    protected override void SetParameters()
    {
        Hp = 3;
        Shot = 5;
        RelativePosition = new Vector3(-0.045f, 0.156f, 0.0f);
        WithdrawalRange = 7.0f;
        MAXSpeed = 2.0f;
    }
}
