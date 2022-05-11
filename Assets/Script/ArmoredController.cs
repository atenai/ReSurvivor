using System.Collections;
using Model;
using UnityEngine;

public class ArmoredController : PatrollerController
{
    [SerializeField] private Transform machineGunSoldier;
    [SerializeField] private ParticleSystem gunParticle;

    protected override void SetParameters()
    {
        Hp = 8;
        Attack = 10;
        Shot = 4;
        RelativePosition = new Vector3(-0.007f, 0.01154f, 0.0f);
        RelativePositionBack = new Vector3(0.066f, 0.0113f, 0.0f);
        MAXSpeed = 1.5f;
        MoveForce = 200.0f;
        EnableTowards = true;
        HasMuzzleFlash = false;
        HasHitEffect = true;
        HitEffectScale = 0.4f;
    }

    protected override void Start()
    {
        base.Start();
        TowardsPlayer = MakeTowardsPlayer(machineGunSoldier);
    }

    protected override IEnumerator RoundFireTimer(int shot,
        FireDirection fireDirection = FireDirection.Horizontal)
    {
        gunParticle.Play();
        yield return StartCoroutine(base.RoundFireTimer(shot, FireDirection.TowardsPlayer));
        gunParticle.Stop();
    }

}
