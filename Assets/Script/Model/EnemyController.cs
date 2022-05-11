using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Model
{
    [RequireComponent(typeof(Rigidbody))]

    public abstract class EnemyController : MonoBehaviour
    {
        /// <summary>
        ///   <para>変数名: パラメーター[既定値]</para>
        ///   <para>hp: 耐久値[5]</para>
        ///   <para>attack: 攻撃力（単発）[20]</para>
        ///   <para>shot: 連射数[1]</para>
        ///   <para>thrust: 銃弾の推力（速度を決める）[350.0f]</para>
        ///   <para>relativePosition: 発射口の相対位置[new Vector3()]</para>
        ///   <para>activeRange: 稼働範囲[18.0f]</para>
        ///   <para>withdrawalRange: 離脱範囲[9.0f]</para>
        ///   <para>turnThreshold: ターン閾値[0.1f]</para>
        ///   <para>maxSpeed: 移動速度[0.0f]</para>
        ///   <para>moveForce: 推力（加速度を決める）[10.0f]</para>
        ///   <para>enableTowards: プレイヤーへ向く機能の有効性[true]</para>
        ///   <para>hasMuzzleFlash: MuzzleFlashエフェクトの有効性[false]</para>
        ///   <para>hasHitEffect: Hitエフェクトの有効性[false]</para>
        /// </summary>
        protected abstract void SetParameters();

        #region SerializeField
        /// <summary>弾のPrefab</summary>
        public GameObject bulletPrefab;

        /// <summary>
        /// 薬莢のPrefab
        /// </summary>
        public GameObject cartridgePrefab;

        /// <summary>
        /// MuzzleFlashエフェクト（左向き）のPrefab
        /// </summary>
        public GameObject muzzleFlashLeftPrefab;

        /// <summary>
        /// MuzzleFlashエフェクト（右向き）のPrefab
        /// </summary>
        public GameObject muzzleFlashRightPrefab;
        
        /// <summary>
        /// HitエフェクトのPrefab
        /// </summary>
        public GameObject hitPrefab;

        /// <summary>
        /// 射撃の効果音
        /// </summary>
        [FormerlySerializedAs("fireSE")] public AudioClip fireSe;

        [SerializeField] private Transform muzzle;

        [SerializeField] [Range(0, float.MaxValue)]
        protected float fireDelay;
        [SerializeField] [Range(0, float.MaxValue)]
        private float singleInterval = 0.1f;
        [SerializeField] [Range(0, float.MaxValue)]
        private float roundInterval = 2.5f;
        #endregion

        #region プロパティ
        private int _hp;
        /// <summary>耐久値</summary>
        public int Hp
        {
            get => _hp;
            protected set
            {
                if (Hp > 0 && value <= 0)
                {
                    OnHPRunOut();
                }

                if (MAXHp is int maxHp && value > MAXHp)
                {
                    _hp = maxHp;
                }
                else
                {
                    _hp = value;
                }


                if (MAXHp.HasValue && Phase is int phase)
                {
                    PhaseSwitch(Hp, phase);
                }
            }
        }

        /// <summary>最大耐久値</summary>
        public int? MAXHp { get; private set; }

        /// <summary>
        /// 現在の段階、直接代入禁止
        /// </summary>
        protected int? Phase { get; set; }

        private Vector3 _relativePosition;
        /// <summary>発射口の相対位置</summary>
        protected Vector3 RelativePosition
        {
            get => DirectionX.x < 0 ? _relativePosition : RelativePositionBack;
            set => _relativePosition = value;
        }

        private Func<Vector3> _relativePositionBack;

        /// <summary>後ろ向き発射口の相対位置</summary>
        protected Vector3 RelativePositionBack
        {
            get => _relativePositionBack?.Invoke() ?? _relativePosition;
            set => _relativePositionBack = () => value;
        }

        protected Vector3 MuzzlePosition => muzzle != null ? muzzle.position : transform.TransformPoint(RelativePosition);


        /// <summary>
        /// 攻撃力（単発）
        /// </summary>
        protected int Attack { get; set; }

        /// <summary>連射数</summary>
        protected int Shot { get; set; }

        /// <summary>連射間隔</summary>
        protected float SingleInterval
        {
            get => singleInterval;
            set => singleInterval = value;
        }

        /// <summary>射撃間隔</summary>
        protected float RoundInterval 
        { 
            get => roundInterval + Random.Range(-0.25f, 0.25f);
            set => roundInterval = value;
        }

        /// <summary>銃弾の推力（速度を決める）</summary>
        protected float Thrust { get; set; }

        /// <summary>稼働範囲</summary>
        protected float ActiveRange { get; set; }

        /// <summary>離脱範囲</summary>
        protected float WithdrawalRange { get; set; }

        /// <summary>移動速度</summary>
        protected float MAXSpeed { get; set; }

        /// <summary>推力（加速度を決める）</summary>
        protected float MoveForce { get; set; }

        /// <summary>ターン閾値</summary>
        protected float TurnThreshold { get; set; }

        /// <summary>
        /// プレイヤーへ向く機能の有効性
        /// </summary>
        protected bool EnableTowards { get; set; }

        /// <summary>
        /// 死亡アニメーションの持続時間
        /// </summary>
        protected float DieAnimationDuration { get; set; }

        private bool _isActive;
        /// <summary>稼働状態</summary>
        protected bool IsActive
        {
            get => _isActive;
            set
            {
                switch (_isActive)
                {
                    case false when value:
                        OnActivate();
                        break;
                    case true when !value:
                        OnDeactivate();
                        break;
                }

                _isActive = value;
            }
        }

        protected float VisibleDistance { get; set; }

        /// <summary>
        /// カメラに写っているか
        /// </summary>
        protected bool IsVisible => Mathf.Abs(Distance.x) < VisibleDistance;

        /// <summary>プレイヤーのGameObject</summary>
        protected GameObject Player { get; set; }

        /// <summary>このGameObjectにアタッチしているRigidbody</summary>
        protected Rigidbody Rigid { get; private set; }

        /// <summary>
        /// このGameObjectにアタッチしているAudioSource
        /// </summary>
        protected AudioSource AudioSource { get; private set; }

        /// <summary>
        /// このGameObjectにアタッチしているAnimator
        /// </summary>
        protected Animator Animator { get; private set; }

        /// <summary>プレイヤーへの変位ベクター</summary>
        protected Vector3 Distance { get; set; }

        /// <summary>プレイヤーへの方向ベクター</summary>
        protected Vector3 Direction => Distance.normalized;
        /// <summary>プレイヤーへの水平方向ベクター</summary>
        protected Vector3 DirectionX => new Vector3(Distance.x, 0.0f).normalized;
        /// <summary>プレイヤーへの垂直方向ベクター</summary>
        protected Vector3 DirectionY => new Vector3(0.0f, Distance.y).normalized;

        /// <summary>水平速度</summary>
        public float VelocityX => Rigid.velocity.x;

        /// <summary>水平速さ</summary>
        public float SpeedX => Mathf.Abs(VelocityX);

        /// <summary>transform.localScale</summary>
        protected Vector3 LocalScale => transform.localScale;

        /// <summary>
        /// HPバーのGameObject
        /// </summary>
        protected GameObject HpBar { get; private set; }

        /// <summary>
        /// HPバーのSlider
        /// </summary>
        protected Slider HpSlider { get; private set; }

        /// <summary>
        /// HPBar/BackgroundのTransform
        /// </summary>
        protected Transform HpBarBackGround { get; private set; }

        /// <summary>
        /// HPBar/Fill Area/FillのTransform
        /// </summary>
        protected Transform HpBarFill { get; private set; }

        /// <summary>
        /// MuzzleFlashエフェクトの有効性
        /// </summary>
        protected bool HasMuzzleFlash;

        /// <summary>
        /// Hitエフェクトの有効性
        /// </summary>
        protected bool HasHitEffect;

        /// <summary>
        /// Hitエフェクトのスケーリング係数
        /// </summary>
        protected float HitEffectScale;

        /// <summary>
        /// 射撃効果音の有無
        /// </summary>
        protected bool HasFireSe => AudioSource != null && fireSe != null;
        #endregion

        protected virtual void Awake()
        {
            // パラメーター既定値
            Hp = 5;
            Attack = 20;
            Shot = 1;
            Thrust = 350.0f;
            RelativePosition = new Vector3();
            ActiveRange = 25.0f;
            WithdrawalRange = 9.0f;
            MAXSpeed = 0;
            MoveForce = 10.0f;
            TurnThreshold = 0.1f;
            VisibleDistance = 17.0f;
            EnableTowards = true;
            HasMuzzleFlash = false;
            HasHitEffect = false;
            HitEffectScale = 1.0f;
            DieAnimationDuration = 0;

            // パラメーター設置
            SetParameters();
            MAXHp = Hp;

            // その他初期化
            Phase = 1;
            _isActive = false;
            HpBar = null;
            TowardsPlayer = MakeTowardsPlayer(transform);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            Rigid = GetComponent<Rigidbody>();
            AudioSource = GetComponent<AudioSource>();
            Animator = GetComponent<Animator>();

            HpBar = transform.Find("EnemyCanvas/HPBar")?.gameObject;
            if (!(HpBar is null))
            {
                HpSlider = HpBar.GetComponent<Slider>();
                HpBarBackGround = HpBar.transform.Find("Background");
                HpBarFill = HpBar.transform.Find("Fill Area/Fill");
            }

            Distance = Player.transform.position - transform.position;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Player != null)
            {
                // プレイヤーへの距離ベクター
                Distance = Player.transform.position - transform.position;
                // isActive = distance.x <= activeRange;

                if (IsActive)
                {
                    if (EnableTowards)
                    {
                        TowardsPlayer();
                    }
                }

            }

            if (HpBar == null) return;
            Debug.Assert(MAXHp != null, nameof(MAXHp) + " != null");
            HpSlider.value = Hp / (float)MAXHp;
        }

        protected virtual void OnTriggerEnter(Collider collision)
        {
            // プレイヤーの銃弾に当たる
            if (IsVisible && collision.gameObject.CompareTag("Bullet"))
            {
                Hp -= 1;
                collision.enabled = false; // Prevent multiple trigger events
                Destroy(collision.gameObject);

                if (!HasHitEffect) return;
                var hitEffect = Instantiate(hitPrefab, collision.transform.position - collision.GetComponent<Rigidbody>().velocity * Time.deltaTime, Quaternion.identity);
                hitEffect.transform.localScale *= HitEffectScale;
                Destroy(hitEffect, 1.0f);
            }
        }

        protected delegate void TowardDelegate();
        protected TowardDelegate TowardsPlayer;

        protected TowardDelegate MakeTowardsPlayer(Transform t = null)
        {
            if (t == null) t = transform;
            var oriEulerAngles = t.eulerAngles;
            
            void TowardDelegate()
            {
                t.eulerAngles = oriEulerAngles;
                if (Direction.x > 0) t.Rotate(new Vector3(0, 180, 0));
            }
            return TowardDelegate;
        }

        /// <summary>
        ///   <para>プレイヤーと一定距離(withdrawalRange ±　turnThreshold)を保つ</para>
        ///   <para>短時間ごとに呼び出す必要あり</para>
        /// </summary>
        protected void AutoMove()
        {
            // 距離が遠すぎると
            if (Mathf.Abs(Distance.x) > WithdrawalRange + TurnThreshold)
            {
                // 最大速度に達していないと
                if (VelocityX * DirectionX.x < MAXSpeed)
                {
                    // プレイヤーの方へ水平推力をかける
                    Rigid.AddForce(DirectionX * MoveForce);
                }
            }
            // 距離が近すぎると
            else if (Mathf.Abs(Distance.x) < WithdrawalRange - TurnThreshold)
            {
                if (VelocityX * -DirectionX.x < MAXSpeed)
                {
                    Rigid.AddForce(-DirectionX * MoveForce);
                }
            }
        }

        /// <summary>単発射撃</summary>
        protected GameObject SingleFire(FireDirection fireDirection = FireDirection.Horizontal)
        {
            var relativeDirection = (Distance - RelativePosition).normalized;

            // 銃弾生成
            var bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<EnemyBulletController>().Attack = Attack;
            // 銃弾推力
            Vector3 thrust;
            switch (fireDirection)
            {
                case FireDirection.Horizontal:
                    thrust = DirectionX * Thrust;
                    break;

                case FireDirection.TowardsPlayer:
                    thrust = relativeDirection * Thrust;

                    var angle = Mathf.Atan(relativeDirection.y / relativeDirection.x);
                    angle = (float)(angle * 180 / Math.PI);
                    bullet.transform.Rotate(Vector3.forward, angle);
                    break;

                case FireDirection.Vertical:
                    thrust = Vector3.down * Thrust;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fireDirection), fireDirection, null);
            }
            bullet.transform.position = MuzzlePosition;
            bullet.GetComponent<Rigidbody>().AddForce(thrust);
            var bulletScale = bullet.transform.localScale;
            bullet.transform.localScale = new Vector3(-DirectionX.x * bulletScale.x, bulletScale.y, bulletScale.z);

            // 薬莢
            try
            {
                var cartridge = Instantiate(cartridgePrefab);
                var position = new Vector3(-0.4f, 0.8f, 0) + transform.position;
                cartridge.transform.position = position;
                var force = new Vector3(0, 100.0f, 300.0f);
                cartridge.GetComponent<Rigidbody>().AddForce(force);
                Destroy(cartridge, 2.0f);
            }
            catch (Exception)
            {
                // ignored
            }

            // MuzzleFlashエフェクト
            if (HasMuzzleFlash)
            {
                var muzzleFlash = Instantiate(DirectionX.x <= 0 ? muzzleFlashLeftPrefab : muzzleFlashRightPrefab, MuzzlePosition, Quaternion.identity);
                Destroy(muzzleFlash, 1.0f);
            }

            // 射撃効果音
            if (HasFireSe)
            {
                AudioSource.PlayOneShot(fireSe);
            }

            return bullet;
        }

        /// <summary>発射方向タイプ</summary>
        protected enum FireDirection
        {
            /// <summary>プレイヤーの中心へ</summary>
            TowardsPlayer,

            /// <summary>プレイヤーの方へ水平に</summary>
            Horizontal,

            /// <summary>
            /// 垂直に落とす
            /// </summary>
            Vertical
        }

        /// <summary>１回の射撃においての連射タイマー</summary>
        /// <param name="shot">連射数</param>
        /// <param name="fireDirection"></param>
        protected virtual IEnumerator RoundFireTimer(int shot, FireDirection fireDirection = FireDirection.Horizontal)
        {
            return RoundFireTimer(shot, SingleInterval, fireDirection);
        }

        protected virtual IEnumerator RoundFireTimer(int shot, float interval,
            FireDirection fireDirection = FireDirection.Horizontal)
        {
            var fired = 0;
            GameObject lastBullet = null;
            var lastPosition = transform.position;
            while (fired < shot)
            {
                // １発撃つ
                var bullet = SingleFire(fireDirection);

                // 後の銃弾は直前の銃弾から均等な間隔を空ける
                if (lastBullet != null)
                {
                    var positionChanged = transform.position - lastPosition;
                    var lastBulletMoved = singleInterval * lastBullet.GetComponent<Rigidbody>().velocity;
                    bullet.transform.position = lastBullet.transform.position - lastBulletMoved + positionChanged;
                }

                fired += 1;
                lastBullet = bullet;
                lastPosition = transform.position;

                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>射撃タイマー</summary>
        /// <param name="delay">delay before first fire in seconds</param>
        protected IEnumerator AutoFireTimer(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            float timePassed = 0;
            while (true)
            {
                if (IsVisible)
                {
                    StartCoroutine(RoundFireTimer(Shot));
                }
                while (timePassed < roundInterval)
                {
                    yield return new WaitForFixedUpdate();
                    timePassed += Time.fixedDeltaTime;
                }
                timePassed -= roundInterval;
            }
        }

        /// <summary>gameObjectの耐久値(hp)が0以下になると呼び出される</summary>
        protected virtual void OnHPRunOut()
        {
            IsActive = false;
            Destroy(HpBar);
            Destroy(gameObject, DieAnimationDuration);
        }

        /// <summary>プレイヤーとの距離がactiveRange以下になると１回だけ呼び出される</summary>
        protected virtual void OnActivate()
        {

        }

        /// <summary>プレイヤーとの距離がactiveRange以上になると１回だけ呼び出される</summary>
        protected virtual void OnDeactivate()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// 段階スイッチ
        /// </summary>
        /// <param name="hp">現在のhp</param>
        /// <param name="phase">現在の段階（初期段階は１）</param>
        protected virtual void PhaseSwitch(int hp, int phase)
        {

        }
    }
}
