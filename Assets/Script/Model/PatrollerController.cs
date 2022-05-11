using System.Collections.Generic;
using Plugin;
using UnityEngine;

namespace Model
{
    public abstract class PatrollerController : EnemyController, IViewable
    {
        private ImtStateMachine<PatrollerController> _stateMachine;
        private FieldOfView _fov;
        private Transform _viewVisualization;
        private Transform _targetFound;

        #region ステートマシン定義
        /// <summary>
        /// 遷移イベントのID
        /// </summary>
        private enum EventID
        {
            Found,
            Lost,
            Die
        }

        /// <summary>
        /// 非アクティブステート
        /// </summary>
        private class InactiveState : ImtStateMachine<PatrollerController>.State
        {

        }

        /// <summary>
        /// 地上にいるステート
        /// </summary>
        private class PatrolState : ImtStateMachine<PatrollerController>.State
        {
            private Coroutine _patrolCoroutine;

            protected internal override void Enter()
            {
                Context.IsActive = false;
                Context.ChangeViewColor(0);
                _patrolCoroutine = Context.StartCoroutine(Context.GetComponent<Patrol>().PatrolInRange());
            }

            protected internal override void Update()
            {

            }

            protected internal override void Exit()
            {
                Context.StopCoroutine(_patrolCoroutine);
            }
        }

        /// <summary>
        /// 空中にいるステート
        /// </summary>
        private class LockOnState : ImtStateMachine<PatrollerController>.State
        {
            private Coroutine _lockOnCoroutine;


            protected internal override void Enter()
            {
                Context.IsActive = true;
                Context.ChangeViewColor(2);
                _lockOnCoroutine = Context.StartCoroutine(Context._fov.LockOn(Context._targetFound));
            }

            protected internal override void Update()
            {
                Context.AutoMove();
            }

            protected internal override void Exit()
            {
                Context._fov.LockOff();
                Context.StopCoroutine(_lockOnCoroutine);
            }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            // ステートマシンの初期化
            _stateMachine = new ImtStateMachine<PatrollerController>(this);

            #region ステートマシン遷移表
            _stateMachine.AddTransition<PatrolState, LockOnState>((int)EventID.Found);
            _stateMachine.AddTransition<LockOnState, PatrolState>((int)EventID.Lost);
            _stateMachine.AddAnyTransition<InactiveState>((int)EventID.Die);
            #endregion

            // 初期ステート
            _stateMachine.SetStartState<PatrolState>();
        }

        protected override void Start()
        {
            base.Start();
            _fov = GetComponent<FieldOfView>();
            _viewVisualization = transform.Find("ViewVisualization");

            _stateMachine.Update();


        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            _stateMachine.Update();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _ = StartCoroutine(AutoFireTimer(fireDelay));
        }

        protected override void OnHPRunOut()
        {
            Destroy(_viewVisualization.gameObject);
            _stateMachine.SendEvent((int)EventID.Die);
            base.OnHPRunOut();
        }

        public void OnFoundVisibleTarget(Transform target)
        {
            _targetFound = target;
            _stateMachine.SendEvent((int)EventID.Found);
        }

        public void OnLostAllTargets()
        {
            _targetFound = null;
            _stateMachine.SendEvent((int)EventID.Lost);
        }

        private void ChangeViewColor(int x)
        {
            if (_viewVisualization == null) return;
            _viewVisualization.GetComponent<MaterialSwitcher>().SelectByIndex(x);
        }

        public List<Transform> VisibleTargets()
        {
            return _fov.visibleTargets;
        }

        public bool FoundAnyTarget()
        {
            return _fov.visibleTargets.Count > 0;
        }

        protected override void OnTriggerEnter(Collider collision)
        {
            base.OnTriggerEnter(collision);
            if (collision.gameObject.CompareTag("Bullet"))
            {
                _fov.AddNoneLost(Player.transform);
            }
        }
    }
}
