using Model;
using UnityEngine;
using UnityEngine.UI;

public class Player3D : MonoBehaviour
{
    //アニメーション
    public Animator anim;//Unityアニメーション用変数
    public bool b_animJump;
    public bool b_animReload;
    public bool b_animDie;
    bool b_animRun;
    bool b_animIdle;
    float IdleTimeDefine = 1.0f;//待機アニメーションへ行こうする時間
    float IdleTime;

    //移動
    public float speed = 4.0f;
    private float moveForce = 50.0f;
    private float speedX => rigid.velocity.x;

    //ジャンプ
    private Rigidbody rigid;
    private float jumpForce = 570.0f;//ジャンプのy軸に加える力
    private float JumpTimeDefine = 1.2f;
    private float JumpTime;//ジャンプの再使用までのロードタイム
    /// <summary>
    /// 重力補正
    /// </summary>
    public float gravityScale = 1.0f;

    //弾
    public GameObject Bullet;

    //弾発射のSE
    public GameObject BulletSEPrefab;
    public float BulletSE_Endtime = 1.0f;

    //弾数
    public static int Magazine;//残弾数
    private int MagazineDefine = 20;
    public static bool b_ReloadTimeActive;//リロードのオン/オフ
    public static float ReloadTime;
    private float ReloadTimeDefine = 1.5f;//リロード時間の固定

    //薬莢
    public GameObject Cartridge;
    public float CartridgeDestroyTime;

    //リロードのSE
    public GameObject ReloadSEPrefab;
    public float ReloadSE_Endtime;

    //右マズルフラッシュエフェクトのプレファブ
    public GameObject RightMuzzleflashEffectPrefab;
    private Vector3 RightMuzzleflashEffectPosition;
    public float RightMuzzleflashEffectDestroyTime = 0.5f;

    //左マズルフラッシュエフェクトのプレファブ
    public GameObject LeftMuzzleflashEffectPrefab;
    private Vector3 LeftMuzzleflashEffectPosition;
    public float LeftMuzzleflashEffectDestroyTime = 0.5f;

    //体力
    private int HP = 100;

    //ゲームオーバータイム
    public static bool b_GameOverTrigger;
    private const float GameOverDelay = 2.0f;


    //プレイヤーダメージSE
    public GameObject PlayerDamageSEPrefab;
    public float PlayerDamageSE_Endtime;
    private float PlayerDamageTime;
    private float PlayerDamageTimeDefine = 1.0f;

    //ダメージエフェクト
    static public bool b_DamageEffect;
    private Image DamageEffectimg;

    //血エフェクトのプレファブ
    public GameObject BloodEffectPrefab;
    private Vector3 BloodEffectPosition;
    public float BloodEffectDestroyTime;

    //救急箱エフェクト
    public bool b_Firstaidkit;
    private Image Firstaidkitimg;

    //ヒールSE
    public GameObject HealSEPrefab;
    public float HealSE_Endtime;

    //ヒールエフェクトのプレファブ
    public GameObject HealEffectPrefab;
    private Vector3 HealEffectPosition;
    public float HealEffectDestroyTime;
    public bool b_HealEffect;
    private float HealTimeDefine = 0.2f;
    private float HealTime;

    //回転
    bool rot = true;

    private IHidable[] hidables;

    //カメラ
    CameraController cameraController;

    private void Start()
    {
        //ジャンプ
        rigid = GetComponent<Rigidbody>();
        JumpTime = JumpTimeDefine;//ジャンプの再使用までのロードタイム

        //体力
        HP = 100;

        //ゲームオーバータイム
        b_GameOverTrigger = false;

        //ダメージエフェクト
        DamageEffectimg = GameObject.Find("PlayerDamageImage").GetComponent<Image>();
        DamageEffectimg.color = Color.clear;
        b_DamageEffect = false;

        //救急箱エフェクト
        Firstaidkitimg = GameObject.Find("FirstaidkitImage").GetComponent<Image>();
        Firstaidkitimg.color = Color.clear;
        //ヒールエフェクト
        b_HealEffect = false;
        HealTime = HealTimeDefine;

        //回転
        rot = true;

        //アニメーション
        anim = GetComponent<Animator>();//アニメーションのコンポーネントを探す
        b_animJump = false;
        b_animReload = false;
        b_animDie = false;
        b_animRun = false;
        b_animIdle = false;
        IdleTimeDefine = 2.0f;

        //弾発射のSE
        BulletSE_Endtime = 1.0f;

        //弾数
        Magazine = MagazineDefine;//残弾数
        b_ReloadTimeActive = false;//リロードのオン/オフ
        ReloadTime = 0.0f;//リロードタイム
        ReloadTimeDefine = 1.5f;

        //マズルフラッシュデストロイタイム
        RightMuzzleflashEffectDestroyTime = 0.5f;
        LeftMuzzleflashEffectDestroyTime = 0.5f;

        hidables = GameObject.Find("Hidable").GetComponentsInChildren<IHidable>();

        //カメラ
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    private void Update()
    {
        if (b_GameOverTrigger) return;
        rigid.velocity += (gravityScale - 1) * Physics.gravity * Time.deltaTime;

        //現在のアニメーション（"Speed"）の値を持ってくる
        float current_speed = anim.GetFloat("Speed");

        //アニメーションの値が１以上なら１にする
        if (1.0f <= current_speed)
        {
            current_speed = 1.0f;
        }

        //アニメーションの値が０以下なら０にする
        if (current_speed <= 0.0f)
        {
            current_speed = 0.0f;
        }

        //移動
        if (Input.GetKey("d") && b_GameOverTrigger == false)
        {
            //待機アニメーション
            b_animIdle = false;
            anim.SetBool("b_Idle", b_animIdle);
            IdleTime = 0.0f;

            //回転
            rot = true;

            if (rot)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (rot == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            b_animRun = true;
            anim.SetFloat("Speed", current_speed + Time.deltaTime * 1.0f);

            //移動
            if (speedX < speed)
            {
                rigid.AddForce(moveForce * Vector3.right);
            }
            //transform.position += transform.forward * speed * Time.deltaTime;

        }
        if (Input.GetKey("a") && b_GameOverTrigger == false)
        {
            //待機アニメーション
            b_animIdle = false;
            anim.SetBool("b_Idle", b_animIdle);
            IdleTime = 0.0f;

            //回転
            rot = false;

            if (rot)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (rot == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            b_animRun = true;
            anim.SetFloat("Speed", current_speed + Time.deltaTime * 1.0f);

            //移動
            if (speedX > -speed)
            {
                rigid.AddForce(moveForce * Vector3.left);
            }
            //transform.position += transform.forward * speed * Time.deltaTime;

        }

        if (b_animRun == false)
        {
            anim.SetFloat("Speed", current_speed - Time.deltaTime * 1.0f);
        }

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && JumpTimeDefine <= JumpTime && b_GameOverTrigger == false)
        {
            //アニメーション
            b_animJump = true;
            anim.SetBool("b_Jump", b_animJump);


            rigid.AddForce(transform.up * jumpForce);
            JumpTime = 0.0f;
        }

        if (JumpTime <= JumpTimeDefine)
        {
            JumpTime += Time.deltaTime;
        }

        if (JumpTime >= 0.1f)
        {
            b_animJump = false;
        }
        anim.SetBool("b_Jump", b_animJump);

        //Debug.Log("ジャンプタイム" + JumpTime);

        //弾
        if ((Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(0)) && (Magazine != 0) && b_ReloadTimeActive == false && b_GameOverTrigger == false)
        {
            //待機アニメーション
            b_animIdle = false;
            anim.SetBool("b_Idle", b_animIdle);
            IdleTime = 0.0f;

            Magazine = Magazine - 1;//残弾数を-1する

            //SEオブジェクトを生成する
            var SE = Instantiate(BulletSEPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(SE, BulletSE_Endtime);//SEをSE_Endtime後削除

            var PositionX = gameObject.transform.position.x;
            var PositionY = gameObject.transform.position.y;
            var PositionZ = gameObject.transform.position.z;
            if (rot)
            {

                //マズルフラッシュエフェクトオブジェクトを生成する	
                RightMuzzleflashEffectPosition = new Vector3(PositionX + 1.75f, PositionY + 1.1f, PositionZ);
                var Effect = Instantiate(RightMuzzleflashEffectPrefab, RightMuzzleflashEffectPosition, Quaternion.identity);
                Destroy(Effect, RightMuzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                //1.newBulletを生成
                //2.newBulletの中でダミーバレットを作成
                //3.ダミーバレットはプレイヤーキャラクターの座標位置から発射される
                //4.数秒後にバレットが敵とのあたり判定をする
                //5.バレットの消失と共にダミーバレットもデストロイされる

                var v3_Position = new Vector3(PositionX + 1.75f, PositionY + 1.1f, PositionZ);
                var newBullet = Instantiate(Bullet, v3_Position, transform.rotation);
                //右方向に飛ばす 
                newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                var v3_CartridgePosition = new Vector3(PositionX + 0.4f, PositionY + 0.8f, PositionZ);
                var newCartridge = Instantiate(Cartridge, v3_CartridgePosition, transform.rotation);
                //右方向に飛ばす 
                newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);//速すぎるとすり抜けてしまう
                newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);//速すぎるとすり抜けてしまう
                Destroy(newCartridge, CartridgeDestroyTime);//DestroyTime後削除
            }
            else if (rot == false)
            {

                //マズルフラッシュエフェクトオブジェクトを生成する	
                LeftMuzzleflashEffectPosition = new Vector3(PositionX - 1.6f, PositionY + 1.0f, PositionZ);
                var Effect = Instantiate(LeftMuzzleflashEffectPrefab, LeftMuzzleflashEffectPosition, Quaternion.identity);
                Destroy(Effect, LeftMuzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                var v3_Position = new Vector3(PositionX - 1.8f, PositionY + 0.9f, PositionZ);
                var newBullet = Instantiate(Bullet, v3_Position, transform.rotation);
                //左方向に飛ばす 
                newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                var v3_CartridgePosition = new Vector3(PositionX + 0.4f, PositionY + 0.8f, PositionZ);
                var newCartridge = Instantiate(Cartridge, v3_CartridgePosition, transform.rotation);
                //右方向に飛ばす 
                newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);//速すぎるとすり抜けてしまう
                newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);//速すぎるとすり抜けてしまう
                Destroy(newCartridge, CartridgeDestroyTime);//DestroyTime後削除
            }

        }

        //リロードシステム
        if (Magazine == 0 || (Magazine != 20 && Input.GetKey(KeyCode.R)) && b_GameOverTrigger == false)
        {
            b_ReloadTimeActive = true;//リロードのオン
        }

        if (b_ReloadTimeActive)//リロードがオンになったら
        {
            if (ReloadTime == 0)
            {
                //待機アニメーション
                b_animIdle = false;
                anim.SetBool("b_Idle", b_animIdle);
                IdleTime = 0.0f;

                //SEオブジェクトを生成する
                var ReloadSE = Instantiate(ReloadSEPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(ReloadSE, ReloadSE_Endtime);//SEをSE_Endtime後削除

                //アニメーション
                b_animReload = true;
                anim.SetBool("b_Reload", b_animReload);
            }
            //リロード中画像
            ReloadTime += Time.deltaTime;//リロードタイムをプラス
            //Debug.Log("リロードタイム" + Riro);
            if (ReloadTimeDefine <= ReloadTime)//リロードタイムが10以上になったら
            {
                Magazine = MagazineDefine;//弾リセット
                ReloadTime = 0.0f;//リロードタイムをリセット
                b_ReloadTimeActive = false;//リロードのオフ
                b_animReload = false;//リロードアニメーションのオフ
                anim.SetBool("b_Reload", b_animReload);
            }
        }
        //リロードシステム

        b_animRun = false;//アニメーションの移動をFalseにする

        //体力
        if (!b_GameOverTrigger && HP <= 0)
        {
            //アニメーション
            b_animDie = true;
            anim.SetBool("b_Die", b_animDie);
            b_GameOverTrigger = true;
            StageSceneController.GameOver(GameOverDelay);
        }

        //プレイヤーダメージSE
        PlayerDamageTime += Time.deltaTime;
        //Debug.Log("プレイヤーダメージSE" + PlayerDamageTime);

        //ダメージエフェクト
        if (b_DamageEffect)
        {
            DamageEffectimg.color = new Color(0.5f, 0f, 0f, 0.5f);
            //血エフェクトオブジェクトを生成する	
            //血エフェクト座標
            BloodEffectPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z);
            var Effect = Instantiate(BloodEffectPrefab, BloodEffectPosition, Quaternion.identity);
            Destroy(Effect, BloodEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
        }

        if (b_DamageEffect == false)
        {
            DamageEffectimg.color = Color.Lerp(DamageEffectimg.color, Color.clear, Time.deltaTime);
        }

        b_DamageEffect = false;

        //救急箱エフェクト
        if (b_Firstaidkit)
        {
            Firstaidkitimg.color = new Color(0f, 0.5f, 0f, 0.5f);

            HealTime = 0.0f;
        }

        if (b_Firstaidkit == false)
        {
            Firstaidkitimg.color = Color.Lerp(Firstaidkitimg.color, Color.clear, Time.deltaTime);
        }

        b_Firstaidkit = false;

        //Debug.Log(HP);

        if (b_HealEffect)
        {
            //ヒールエフェクトオブジェクトを生成する	
            //ヒールエフェクト座標
            HealEffectPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z - 1.0f);

            var HealEffect = Instantiate(HealEffectPrefab, HealEffectPosition, Quaternion.identity);
            Destroy(HealEffect, HealEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
        }

        if (HealTime <= HealTimeDefine)
        {
            b_HealEffect = true;
        }


        if (HealTimeDefine <= HealTime)
        {
            b_HealEffect = false;
        }

        HealTime += Time.deltaTime;
        //Debug.Log("ヒールタイム" + HealTime);

        //待機アニメーション
        if (IdleTimeDefine <= IdleTime)
        {
            b_animIdle = true;
            anim.SetBool("b_Idle", b_animIdle);
            IdleTime = 0.0f;
        }

        if (b_animIdle == false)
        {
            IdleTime += Time.deltaTime;
            //Debug.Log(IdleTime);
        }

        foreach (var hidable in hidables)
        {
            if (Input.GetKeyDown(hidable.HideKey()) && hidable.IsAccessable(gameObject))
            {
                hidable.Hide(gameObject);
                break;
            }
        }
    }

    //↓Set関数

    public void SetPlayerDamage(int Damage)
    {
        HP = HP - Damage;
        if (HP <= 0)
        {
            HP = 0;
        }
    }

    public void SetPlayerHeal(int Heal)
    {
        HP = HP + Heal;
        if (100 <= HP)
        {
            HP = 100;
        }
    }

    //↓Get関数

    public int GetPlayerHP()
    {
        return HP;
    }

    public float GetPlayerPositionX()
    {
        return this.transform.position.x;
    }

    //↓当たり判定
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") && b_GameOverTrigger == false || other.CompareTag("Mine") && b_GameOverTrigger == false)
        {
            if (PlayerDamageTimeDefine <= PlayerDamageTime)
            {
                //SEオブジェクトを生成する
                var SE = Instantiate(PlayerDamageSEPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(SE, PlayerDamageSE_Endtime);//SEをSE_Endtime後削除

                PlayerDamageTime = 0.0f;
            }

            b_DamageEffect = true;

            //cameraController.Shake(0.25f, 0.1f);

            //SetPlayerDamage(50);
        }

        if (other.CompareTag("First aid kit") && HP < 100 && b_GameOverTrigger == false)
        {
            //SEオブジェクトを生成する
            var SE = Instantiate(HealSEPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(SE, HealSE_Endtime);//SEをSE_Endtime後削除

            b_Firstaidkit = true;
            SetPlayerHeal(100);
        }
    }
}
