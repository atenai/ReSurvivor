using System.Collections;
using Model;
using Plugin;
using UnityEngine;

public class BossController : EnemyController
{
    protected override void SetParameters()
    {
        Hp = 20;
        RelativePosition = new Vector3(0, 0.49f, 0.63f);
        RelativePositionBack = new Vector3(0, 0.619f, 0.64f);
        MAXSpeed = 3.0f;
        Shot = 3;
        Thrust = 400.0f;
        WithdrawalRange = 10.0f;
        TurnThreshold = 3.0f;
        HasMuzzleFlash = true;
        HasHitEffect = true;
    }

    #region プロパティ
    /// <summary>
    /// 跳ぶ時の弾力
    /// </summary>
    protected float JumpForce { get; set; }

    /// <summary>
    /// 地面に着いてから跳ぶまでの時間
    /// </summary>
    protected float JumpDelay { get; set; }

    /// <summary>
    /// 跳んでから空中発砲までの時間
    /// </summary>
    protected float JumpFireDelay { get; set; }

    /// <summary>
    /// 空中発砲の間隔
    /// </summary>
    protected float JumpFireInterval { get; set; }

    /// <summary>
    /// 空中発砲の連射数
    /// </summary>
    protected int JumpShot { get; set; }

    /// <summary>
    /// 距離変更の間隔
    /// </summary>
    protected float ChangeInterval { get; set; }

    /// <summary>
    /// 被弾可能の間隔
    /// </summary>
    protected float HitInterval { get; set; }


    /// <summary>
    /// 垂直速さ
    /// </summary>
    protected float VelocityY { get => Rigid.velocity.y; }
    #endregion

    #region privateフィールド
    /// <summary>
    /// ステートマシン
    /// </summary>
    private ImtStateMachine<BossController> _stateMachine;
    #endregion

    #region ステートマシン定義
    /// <summary>
    /// 遷移イベントのID
    /// </summary>
    private enum EventID
    {
        Jump,
        TouchGround,
        Activate,
        Deactivate
    }

    /// <summary>
    /// 非アクティブステート
    /// </summary>
    private class InactiveState : ImtStateMachine<BossController>.State
    {

    }

    /// <summary>
    /// 地上にいるステート
    /// </summary>
    private class OnGroundState : ImtStateMachine<BossController>.State
    {
        private bool _hasJumped;
        private Coroutine _groundFireTimer;
        private Coroutine _waitToJump;

        protected internal override void Enter()
        {
            Context.IsActive = true;

            // 地上の発砲を開始
            _groundFireTimer = Context.StartCoroutine(Context.GroundFireTimer());

            // Phase2以上であると
            if (Context.Phase >= 2)
            {
                // 跳ぶのを待つ
                _waitToJump = Context.StartCoroutine(Context.WaitToJump());
            }
        }

        protected internal override void Update()
        {
            // 自動移動
            Context.AutoMove();
        }

        protected internal override void Exit()
        {
            // 地上の発砲を停止
            Context.StopCoroutine(_groundFireTimer);
            // 跳び待ちを停止
            if (_waitToJump != null)
            {
                Context.StopCoroutine(_waitToJump);
            }
        }
    }

    /// <summary>
    /// 空中にいるステート
    /// </summary>
    private class InAirState : ImtStateMachine<BossController>.State
    {
        private bool _isOnGround;
        private Coroutine _jumpFireTimer;

        protected internal override void Enter()
        {
            _isOnGround = false;

            // 空中の発砲を開始
            _jumpFireTimer = Context.StartCoroutine(Context.JumpFireTimer());
        }

        protected internal override void Update()
        {
            // 二回連続で垂直速度が０であると地面に着くイベントを送信
            if (_isOnGround && Context.VelocityY == 0)
            {
                _ = stateMachine.SendEvent((int)EventID.TouchGround);
            }
            _isOnGround = Context.VelocityY == 0;
        }

        protected internal override void Exit()
        {
            // 空中の発砲を停止
            Context.StopCoroutine(_jumpFireTimer);
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        // 新規パラメーター
        JumpForce = 400.0f;
        JumpDelay = 5.0f;
        JumpFireDelay = 0.5f;
        JumpFireInterval = 0.1f;
        JumpShot = 5;
        ChangeInterval = 1.0f;
        HitInterval = 1.0f;

        // ステートマシンの初期化
        _stateMachine = new ImtStateMachine<BossController>(this);

        #region ステートマシン遷移表
        _stateMachine.AddTransition<InactiveState, OnGroundState>((int)EventID.Activate);
        _stateMachine.AddTransition<OnGroundState, InAirState>((int)EventID.Jump);
        _stateMachine.AddTransition<InAirState, OnGroundState>((int)EventID.TouchGround);

        _stateMachine.AddAnyTransition<InactiveState>((int)EventID.Deactivate);
        #endregion

        // 初期ステート
        _stateMachine.SetStartState<InactiveState>();
    }

    protected override void Start()
    {
        base.Start();

        _stateMachine.Update();

        _stateMachine.SendEvent((int)EventID.Activate);
    }

    protected override void Update()
    {
        base.Update();

        _stateMachine.Update();

        Animator.SetParameters(AnimatorState.Run);
    }

    protected override void OnDeactivate()
    {
        _stateMachine.SendEvent((int)EventID.Deactivate);
    }

    #region 段階制御
    protected override void PhaseSwitch(int hp, int phase)
    {
        switch (phase)
        {
            case 1:
                if (hp <= MAXHp * 3 / 4)
                {
                    OnPhase2Enter();
                    Phase = 2;
                }
                goto case 2;
            case 2:
                if (hp <= MAXHp * 1 / 2)
                {
                    OnPhase3Enter();
                    Phase = 3;
                }
                break;
        }
    }

    /// <summary>
    /// Phase１からPhase２に変更する時１回だけ呼び出される
    /// </summary>
    private void OnPhase2Enter()
    {
        //StartCoroutine(ChangeColor(new Color(255 / 255.0f, 66 / 255.0f, 66 / 255.0f), 1.0f));
        RoundInterval = 2.0f;
        StartCoroutine(WaitToJump(1.0f));
    }

    /// <summary>
    /// Phase２からPhase３に変更する時１回だけ呼び出される
    /// </summary>
    private void OnPhase3Enter()
    {
        MoveForce = 15.0f;
        MAXSpeed = 8.0f;
        StartCoroutine(DistanceRandomizer(ChangeInterval));
        JumpDelay = 2.5f;
    }
    #endregion

    /// <summary>
    /// jumpDelay後上へ跳び、跳ぶイベントを送信
    /// </summary>
    protected IEnumerator WaitToJump()
    {
        Debug.Log("<color=red>①jumpDelay後上へ跳び、跳ぶイベントを送信</color>");
        return WaitToJump(JumpDelay);
    }
    /// <summary>
    /// jumpDelay後上へ跳び、跳ぶイベントを送信
    /// </summary>
    protected IEnumerator WaitToJump(float jumpDelay)
    {
        Debug.Log("<color=red>②jumpDelay後上へ跳び、跳ぶイベントを送信</color>");
        yield return new WaitForSeconds(jumpDelay);
        Rigid.AddForce(JumpForce * Vector3.up);
        _ = _stateMachine.SendEvent((int)EventID.Jump);
    }

    /// <summary>
    /// 空中の発砲タイマー
    /// </summary>
    protected IEnumerator JumpFireTimer()
    {
        Debug.Log("<color=blue>③空中の発砲タイマー</color>");
        return JumpFireTimer(JumpFireDelay, JumpFireInterval);
    }
    /// <summary>
    /// 空中の発砲タイマー
    /// </summary>
    protected IEnumerator JumpFireTimer(float jumpFireDelay, float jumpFireInterval)
    {
        Debug.Log("<color=blue>④空中の発砲タイマー</color>");
        yield return new WaitForSeconds(jumpFireDelay);//ここが問題かもしれない
        yield return RoundFireTimer(JumpShot, jumpFireInterval, FireDirection.TowardsPlayer);
    }

    /// <summary>
    /// 地上の発砲タイマー
    /// </summary>
    protected IEnumerator GroundFireTimer()
    {
        Debug.Log("<color=yellow>⑤地上の発砲タイマー</color>");
        return GroundFireTimer(RoundInterval);
    }
    /// <summary>
    /// 地上の発砲タイマー
    /// </summary>
    protected IEnumerator GroundFireTimer(float roundInterval)
    {
        Debug.Log("<color=yellow>⑥地上の発砲タイマー</color>");
        while (true)
        {
            yield return RoundFireTimer(Shot);
            yield return new WaitForSeconds(roundInterval);
        }
    }

    /// <summary>
    /// ランダムの距離を維持
    /// </summary>
    /// <param name="changeInterval">距離変更の間隔</param>
    protected IEnumerator DistanceRandomizer(float changeInterval)
    {
        Debug.Log("<color=green>⑦ランダムの距離を維持</color>");
        while (true)
        {
            TurnThreshold = 1.0f;
            WithdrawalRange = Random.Range(6.0f, 12.0f);
            yield return new WaitForSeconds(changeInterval);
        }
    }

    protected override void OnHPRunOut()
    {
        Destroy(transform.parent.Find("GoalBlocker").gameObject);
        base.OnHPRunOut();
        StageSceneController.IsBossAlive = false;
    }
}
