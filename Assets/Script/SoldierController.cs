using Model;
using UnityEngine;

public class SoldierController : PatrollerController
{
    protected override void SetParameters()
    {
        Hp = 3;
        RelativePosition = new Vector3(0, 0.37f, 0.66f);
        RelativePositionBack = new Vector3(0, 0.51f, 0.67f);
        WithdrawalRange = 6.0f;
        MAXSpeed = 3.0f;
        HasMuzzleFlash = true;
        HasHitEffect = true;
    }

    protected override void Update()
    {
        base.Update();
        Animator.SetParameters(AnimatorState.Run);
        
    }

    protected override void OnHPRunOut()
    {
        DieAnimationDuration = 1.0f;
        Animator.SetParameters(AnimatorState.Die);
        base.OnHPRunOut();
    }
}
