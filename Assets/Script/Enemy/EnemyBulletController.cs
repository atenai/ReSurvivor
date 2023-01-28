using UnityEngine;
using UnityEngine.Serialization;

public class EnemyBulletController : MonoBehaviour
{
    /// <summary>
    ///   <para>変数名: パラメーター[既定値]</para>
    ///   <para>attack: 攻撃力[20]</para>
    ///   <para>duration: 存続時間[5.0f]</para>
    ///   <para>canBeDestroyed: プレイヤーの弾による破壊[false]</para>
    /// </summary>
    protected virtual void SetParameters() { }

    /// <summary>
    /// 爆発エフェクトのPrefab
    /// </summary>
    public GameObject explosionPrefab;

    /// <summary>
    /// エフェクトの再生速度[標準速度：1.0f]
    /// </summary>
    public float effectSpeed = 1.0f;

    /// <summary>
    /// エフェクトのスケーリング係数
    /// </summary>
    public float effectScale = 1.0f;

    /// <summary>
    /// 爆発効果音
    /// </summary>
    [FormerlySerializedAs("explosionSE")] public AudioClip explosionSe;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int Attack { get; set; }

    /// <summary>
    /// このGameObjectにアタッチしているAudioSource
    /// </summary>
    protected AudioSource AudioSource { get; private set; }

    /// <summary>
    /// 爆発エフェクトの有無
    /// </summary>
    protected bool HasExplosionEffect => explosionPrefab != null;

    /// <summary>
    /// 爆発効果音の有無
    /// </summary>
    protected bool HasExplosionSe => explosionSe != null;

    /// <summary>
    /// 存続時間
    /// </summary>
    protected float Duration { get; set; }

    /// <summary>
    /// プレイヤーの弾による破壊
    /// </summary>
    protected bool CanBeDestroyed { get; set; }

    protected virtual void Awake()
    {
        Attack = 20;
        Duration = 5.0f;
        CanBeDestroyed = false;

        SetParameters();
    }

    protected virtual void Start()
    {
        AudioSource = GetComponent<AudioSource>();

        Destroy(gameObject, Duration);
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                if (Player.singletonInstance == null) break;
                //プレイヤーのHPを減らす
                Player.singletonInstance.SetPlayerDamage(Attack);
                DestroyWithEffect(gameObject);
                break;

            case "Kabe":
                DestroyWithEffect(gameObject);
                break;

            case "Bullet":
                if (CanBeDestroyed)
                {
                    DestroyWithEffect(gameObject);
                }
                break;
        }
    }

    /// <summary>
    /// <para>エフェクトを再生</para>
    /// <para>エフェクトのPrefab指定がない場合、nullを返す</para>
    /// </summary>
    /// <returns></returns>
    protected GameObject PlayEffect()
    {
        GameObject explosionEffect = null;
        if (HasExplosionEffect)
        {
            explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionEffect.transform.localScale *= effectScale;
            var main = explosionEffect.GetComponent<ParticleSystem>().main;
            main.simulationSpeed = effectSpeed;
            Destroy(explosionEffect, 1.0f);
        }
        return explosionEffect;
    }

    /// <summary>
    /// <para>効果音を再生</para>
    /// </summary>
    protected void PlaySe()
    {
        if (HasExplosionSe)
        {
            AudioSource.PlayClipAtPoint(explosionSe, transform.position);
        }
    }

    /// <summary>
    /// エフェクト付きのDestroy
    /// </summary>
    /// <param name="gameObject"></param>
    protected void DestroyWithEffect(GameObject gameObject)
    {
        Destroy(gameObject);
        PlayEffect();
        PlaySe();
    }
}
