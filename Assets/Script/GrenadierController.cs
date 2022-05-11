using System.Collections;
using Model;
using UnityEngine;

public class GrenadierController : EnemyController
{
    [SerializeField] private GameObject grenade;
    /// <summary>
    /// 手榴弾のPrefab
    /// </summary>
    [SerializeField] private GameObject grenadePrefab;

    /// <summary>
    /// 投擲目標（Playerとの距離）
    /// </summary>
    [SerializeField] private float throwTo = 5.0f;

    /// <summary>
    /// 投擲開始の水平距離
    /// </summary>
    protected float ThrowRange;

    /// <summary>
    /// 投擲開始の水平位置
    /// </summary>
    protected float ThrowPosition => Player.transform.position.x + ThrowRange;

    /// <summary>
    /// 退場の水平距離
    /// </summary>
    protected float ExitRange;

    /// <summary>
    /// 退場の水平位置
    /// </summary>
    protected float ExitPosition => Player.transform.position.x + ExitRange;

    private int _throwProcess;
    private float _grenadeYMax;

    protected override void SetParameters()
    {
        RelativePosition = new Vector3(-0.36f, 0.6f, 0.0f);
        MAXSpeed = 4.0f;
        HasHitEffect = true;
        EnableTowards = false;
    }

    protected override void Awake()
    {
        base.Awake();
        ThrowRange = 14.0f;
        ExitRange = 20.0f;
        _throwProcess = 0;
        _grenadeYMax = 5.0f;
    }

    protected override void Update()
    {
        base.Update();
        switch (_throwProcess)
        {
            case 0:
                if (MoveTo(ThrowPosition))
                {
                    _ = StartCoroutine(ThrowDelayTimer(1.6f));
                    _throwProcess = 1;
                }
                break;
            case 2:
                if (MoveTo(ExitPosition))
                {
                    Destroy(gameObject);
                }
                break;
        }
        Animator.SetParameters(AnimatorState.Run);
    }

    /// <summary>
    /// 水平座標xまで移動
    /// </summary>
    /// <param name="x"></param>
    protected bool MoveTo(float x)
    {
        var distance = x - transform.position.x;
        if (Mathf.Abs(distance) > 0.1f)
        {
            
            if (SpeedX < MAXSpeed)
            {
                Rigid.AddForce(Vector3.right * (MoveForce * distance) / Mathf.Abs(distance));
            }
            
            return false;
        }

        return true;
    }

    protected IEnumerator ThrowDelayTimer(float delay)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Animator.SetParameters(AnimatorState.Throw);
        grenade.SetActive(true);
        yield return new WaitForSeconds(delay / 2);
        Destroy(grenade);
        ThrowGrenade();
        yield return new WaitForSeconds(delay / 2);
        _throwProcess = 2;
        transform.localScale = new Vector3(LocalScale.x, LocalScale.y, -LocalScale.z);
        MoveForce *= 1.5f;
        MAXSpeed *= 3;
    }

    /// <summary>
    /// 手榴弾を投擲
    /// </summary>
    /// <returns></returns>
    protected void ThrowGrenade()
    {
        var obj = Instantiate(grenadePrefab);

        var position = transform.position + RelativePosition;
        obj.transform.position = position;

        var t1 = Mathf.Sqrt((_grenadeYMax - position.y) * 2 / 9.8f);
        var t2 = Mathf.Sqrt(2 * _grenadeYMax / 9.8f);
        var t = t1 + t2;
        var vy = 9.8f * t1;
        var vx = (Player.transform.position.x
                  // + player.GetComponent<Rigidbody>().velocity.x * t
                  + throwTo
                  - position.x) / t;
        obj.GetComponent<Rigidbody>().velocity = new Vector3(vx, vy, 0.0f);
    }
}
