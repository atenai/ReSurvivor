using System;
using System.Collections;
using Model;
using UnityEngine;
using Object = UnityEngine.Object;

public class BomberController : EnemyController
{
    protected override void SetParameters()
    {
        Shot = 5;
        SingleInterval = 1.0f;
        RoundInterval = 2.5f;
        Thrust = 0.0f;
        RelativePosition = new Vector3(0.69f, -0.74f, 0.0f);
        MAXSpeed = 3.0f;
        MoveForce = 800.0f;
        EnableTowards = false;
    }

    /// <summary>
    /// 爆弾投下の開始距離
    /// </summary>
    protected float BombDropDistance;

    private const float DestroyTime = 3.0f;
    private const float DistanceToDestroy = 15.0f;

    protected override void Awake()
    {
        base.Awake();
        BombDropDistance = 10.0f;
    }

    protected override void Start()
    {
        base.Start();
        _ = StartCoroutine(BombDropTimer(SingleInterval, Shot));
        StartCoroutine(Destroy(gameObject, DestroyTime, () => Distance.x > DistanceToDestroy));
    }

    protected override void Update()
    {
        base.Update();
        AutoFly();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        Destroy(gameObject);
    }

    /// <summary>
    /// 飛行制御
    /// </summary>
    protected void AutoFly()
    {
        if (SpeedX < MAXSpeed)
        {
            Rigid.AddForce(Vector3.left * MoveForce);
        }
    }

    /// <summary>
    /// 爆弾投下タイマー
    /// </summary>
    /// <param name="singleInterval"></param>
    /// <param name="shot"></param>
    /// <returns></returns>
    protected IEnumerator BombDropTimer(float singleInterval, int shot)
    {
        yield return new WaitUntil(() => Mathf.Abs(Distance.x) < BombDropDistance);
        var fired = 0;
        while (fired < shot)
        {
            SingleFire(FireDirection.Vertical);
            fired += 1;
            yield return new WaitForSeconds(singleInterval);
        }
    }

    private static IEnumerator Destroy(Object obj, float delay, Func<bool> cond)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            var t = 0.0f;
            while (cond() && t < delay)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            if (t >= delay) break;
        }
        Destroy(obj);
    }
}
