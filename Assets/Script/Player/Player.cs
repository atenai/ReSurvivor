﻿using Model;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    //シングルトンで作成（ゲーム中に１つのみにする）
    public static Player singletonInstance = null;

    //アニメーション
    [SerializeField] Animator animator;//Unityアニメーション用変数
    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }
    bool isAnimJump = false;
    bool isAnimReload = false;
    bool isAnimDie = false;
    bool isAnimationMove = false;
    bool isAnimIdle = false;
    readonly float idleTimeDefine = 2.0f;//待機アニメーションへ行こうする時間
    float idleTime;
    float animationCurrentPlayerMoveSpeed;

    //移動
    float speed = 4.0f;
    float moveForce = 50.0f;//リジッドボディによる移動
    float speedX => rb.velocity.x;//リジッドボディによる移動

    //ジャンプ
    [SerializeField] Rigidbody rb;
    float jumpForce = 570.0f;//ジャンプのy軸に加える力
    readonly float jumpLoadTimeDefine = 1.2f;
    float jumpLoadTime;//ジャンプの再使用までのロードタイム
    /// <summary>
    /// 重力補正
    /// </summary>
    public float gravityScale = 2.0f;

    //弾
    public GameObject bullet;

    //弾発射のSE
    public GameObject bulletSEPrefab;
    float bulletSEDestroyTime = 1.0f;

    //弾数
    int magazine;//残弾数
    public int Magazine => magazine;
    readonly int magazineDefine = 20;
    bool isReloadTimeActive = false;//リロードのオン・オフ
    public bool IsReloadTimeActive => isReloadTimeActive;
    float reloadTime = 0.0f;
    readonly float reloadTimeDefine = 1.5f;//リロード時間の固定

    //薬莢
    public GameObject cartridge;
    float cartridgeDestroyTime = 1.0f;

    //リロードのSE
    public GameObject reloadSEPrefab;
    float reloadSEDestroyTime = 1.0f;

    //右マズルフラッシュエフェクトのプレファブ
    public GameObject rightMuzzleflashEffectPrefab;
    float muzzleflashEffectDestroyTime = 0.5f;

    //左マズルフラッシュエフェクトのプレファブ
    public GameObject leftMuzzleflashEffectPrefab;

    //体力
    int hp = 100;
    public int HP => hp;

    //ゲームオーバー
    [NonSerialized] public bool isGameOverTrigger = false;
    public readonly float gameOverDelay = 2.0f;

    //プレイヤーダメージSE
    public GameObject playerDamageSEPrefab;
    float playerDamageSEDestroyTime = 1.0f;
    float playerDamageTime;
    float playerDamageTimeDefine = 1.0f;

    //ダメージ画像
    bool isDamage = false;

    //血エフェクト
    public GameObject bloodEffectPrefab;
    float bloodEffectDestroyTime = 1.0f;

    //ヒール画像
    bool isHeal;

    //ヒールSE
    public GameObject healSEPrefab;
    float healSEDestroyTime = 1.0f;

    //ヒールエフェクトのプレファブ
    [SerializeField] GameObject healEffectPrefab;
    float healEffectDestroyTime = 1.0f;
    bool isHealEffect = false;
    readonly float healTimeDefine = 0.2f;
    float healTime;

    //回転
    bool isRotRight = true;

    IHidable[] hidables;

    //カメラ
    [SerializeField] CameraController cameraController;

    //広告
    [SerializeField] AdsInterstitial adsInterstitial;

    void Awake()
    {
        //staticな変数instanceはメモリ領域は確保されていますが、初回では中身が入っていないので、中身を入れます。
        if (singletonInstance == null)
        {
            singletonInstance = this;//thisというのは自分自身のインスタンスという意味になります。この場合、Playerのインスタンスという意味になります。
        }
        else
        {
            Destroy(this.gameObject);//中身がすでに入っていた場合、自身のインスタンスがくっついているゲームオブジェクトを破棄します。
        }
    }

    void Start()
    {
        InitJump();

        InitHeal();

        InitMagazine();

        hidables = GameObject.Find("Hidable").GetComponentsInChildren<IHidable>();
    }

    /// <summary>
    /// ジャンプの初期化処理
    /// </summary>
    void InitJump()
    {
        jumpLoadTime = jumpLoadTimeDefine;//ジャンプの再使用までのロードタイム
    }

    /// <summary>
    /// ヒールエフェクトの初期化処理
    /// </summary>
    void InitHeal()
    {
        healTime = healTimeDefine;
    }

    /// <summary>
    /// 弾数の初期化処理
    /// </summary>
    void InitMagazine()
    {
        magazine = magazineDefine;//残弾数
    }

    void Update()
    {
        if (isGameOverTrigger == true)
        {
            return;
        }

        rb.velocity = rb.velocity + (gravityScale - 1) * Physics.gravity * Time.deltaTime;

        MoveBefoerUpdateSystem();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR//ビルドセッティングがWindowsまたはUnityエディターだった場合の時のみ中身を処理する
        MoveKeyboard();
#endif
#if UNITY_ANDROID//ビルドセッティングがAndroidだった場合の時のみ中身を処理する
        MoveLeftStick();
#endif

        MoveAfterUpdateSystem();

        Idle();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        JumpKeyboard();
#endif
        JumpUpdateSystem();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        ShootKeyboard();
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        ReloadKeyboard();
#endif
        ReloadUpdateSystem();

        HPSystem();

        Hide();
    }

    void MoveBefoerUpdateSystem()
    {
        //現在のアニメーション（"Speed"）の値を持ってくる
        animationCurrentPlayerMoveSpeed = animator.GetFloat("f_CurrentPlayerMoveSpeed");

        //アニメーションの値が1以上なら1にする
        if (1.0f <= animationCurrentPlayerMoveSpeed)
        {
            animationCurrentPlayerMoveSpeed = 1.0f;
        }

        //アニメーションの値が0以下なら0にする
        if (animationCurrentPlayerMoveSpeed <= 0.0f)
        {
            animationCurrentPlayerMoveSpeed = 0.0f;
        }
    }

    //android用Move関数
    void MoveLeftStick()
    {
        var current = Gamepad.current;

        if (current == null)
        {
            return;
        }

        var leftStickValue = current.leftStick.x.ReadValue();
        //Debug.Log("xの移動量 : " + leftStickValue);

        if (0.1f <= leftStickValue)
        {
            //待機アニメーション
            isAnimIdle = false;
            animator.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

            //回転
            isRotRight = true;

            if (isRotRight == true)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (isRotRight == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            animator.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //リジッドボディによる移動
            if (speedX < speed)
            {
                rb.AddForce(moveForce * Vector3.right);
            }
        }

        if (leftStickValue <= -0.1f)
        {
            //待機アニメーション
            isAnimIdle = false;
            animator.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

            //回転
            isRotRight = false;

            if (isRotRight == true)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (isRotRight == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            animator.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //リジッドボディによる移動
            if (speedX > -speed)
            {
                rb.AddForce(moveForce * Vector3.left);
            }
        }
    }

    void MoveKeyboard()
    {
        if (Input.GetKey("d"))
        {
            //待機アニメーション
            isAnimIdle = false;
            animator.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

            //回転
            isRotRight = true;

            if (isRotRight == true)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (isRotRight == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            animator.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //リジッドボディによる移動
            if (speedX < speed)
            {
                rb.AddForce(moveForce * Vector3.right);
            }
        }

        if (Input.GetKey("a"))
        {
            //待機アニメーション
            isAnimIdle = false;
            animator.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

            //回転
            isRotRight = false;

            if (isRotRight == true)
            {
                // y軸を軸にして90度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 90, 0);
                transform.rotation = rot;
            }
            else if (isRotRight == false)
            {
                // y軸を軸にして270度、回転させるQuaternionを作成（変数をrotとする）
                var rot = Quaternion.Euler(0, 270, 0);
                transform.rotation = rot;
            }

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            animator.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //リジッドボディによる移動
            if (speedX > -speed)
            {
                rb.AddForce(moveForce * Vector3.left);
            }
        }
    }

    void MoveAfterUpdateSystem()
    {
        if (isAnimationMove == false)
        {
            //移動アニメーションを徐々に「立ち」状態にする
            animator.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed - Time.deltaTime * 1.0f);
        }

        isAnimationMove = false;//アニメーションの移動をFalseにする
    }

    void Idle()
    {
        //待機アニメーション
        if (idleTimeDefine <= idleTime)
        {
            isAnimIdle = true;
            animator.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;
        }

        if (isAnimIdle == false)
        {
            idleTime += Time.deltaTime;
        }
    }

    //android用Jump関数
    public void JumpTouchButton()
    {
        if (jumpLoadTimeDefine <= jumpLoadTime)
        {
            //アニメーション
            isAnimJump = true;
            animator.SetBool("b_Jump", isAnimJump);

            rb.AddForce(transform.up * jumpForce);
            jumpLoadTime = 0.0f;
        }
    }

    void JumpKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpLoadTimeDefine <= jumpLoadTime)
            {
                //アニメーション
                isAnimJump = true;
                animator.SetBool("b_Jump", isAnimJump);

                rb.AddForce(transform.up * jumpForce);
                jumpLoadTime = 0.0f;
            }
        }
    }

    void JumpUpdateSystem()
    {
        if (jumpLoadTime <= jumpLoadTimeDefine)
        {
            jumpLoadTime += Time.deltaTime;
        }

        if (jumpLoadTime >= 0.1f)
        {
            isAnimJump = false;
        }
        animator.SetBool("b_Jump", isAnimJump);
    }

    //android用Shoot関数
    public void ShootTouchButton()
    {
        if (magazine != 0)
        {
            if (isReloadTimeActive == false)
            {
                //待機アニメーション
                isAnimIdle = false;
                animator.SetBool("b_Idle", isAnimIdle);
                idleTime = 0.0f;

                magazine = magazine - 1;//残弾数を-1する

                //SEオブジェクトを生成する
                UnityEngine.GameObject se = Instantiate(bulletSEPrefab, this.transform.position, Quaternion.identity);
                Destroy(se, bulletSEDestroyTime);//SEをSE_Endtime後削除

                if (isRotRight == true)
                {

                    //マズルフラッシュエフェクトオブジェクトを生成する	
                    Vector3 rightMuzzleflashEffectPosition = new Vector3(this.transform.position.x + 1.75f, this.transform.position.y + 1.1f, this.transform.position.z);
                    UnityEngine.GameObject newEffect = Instantiate(rightMuzzleflashEffectPrefab, rightMuzzleflashEffectPosition, Quaternion.identity);
                    Destroy(newEffect, muzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                    //弾を生成する
                    UnityEngine.Vector3 rightBulletPosition = new Vector3(this.transform.position.x + 1.75f, this.transform.position.y + 1.1f, this.transform.position.z);
                    UnityEngine.GameObject newBullet = Instantiate(bullet, rightBulletPosition, this.transform.rotation);
                    newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                    //薬莢を生成する
                    UnityEngine.Vector3 rightCartridgePosition = new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.8f, this.transform.position.z);
                    UnityEngine.GameObject newCartridge = Instantiate(cartridge, rightCartridgePosition, this.transform.rotation);
                    newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);
                    newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);
                    Destroy(newCartridge, cartridgeDestroyTime);//DestroyTime後削除
                }
                else if (isRotRight == false)
                {

                    //マズルフラッシュエフェクトオブジェクトを生成する	
                    Vector3 leftMuzzleflashEffectPosition = new Vector3(this.transform.position.x - 1.6f, this.transform.position.y + 1.0f, this.transform.position.z);
                    UnityEngine.GameObject newEffect = Instantiate(leftMuzzleflashEffectPrefab, leftMuzzleflashEffectPosition, Quaternion.identity);
                    Destroy(newEffect, muzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                    //弾を生成する
                    UnityEngine.Vector3 leftBulletPosition = new Vector3(this.transform.position.x - 1.8f, this.transform.position.y + 0.9f, this.transform.position.z);
                    UnityEngine.GameObject newBullet = Instantiate(bullet, leftBulletPosition, this.transform.rotation);
                    newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                    //薬莢を生成する
                    UnityEngine.Vector3 leftCartridgePosition = new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.8f, this.transform.position.z);
                    UnityEngine.GameObject newCartridge = Instantiate(cartridge, leftCartridgePosition, this.transform.rotation);
                    newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);
                    newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);
                    Destroy(newCartridge, cartridgeDestroyTime);//DestroyTime後削除
                }
            }
        }
    }

    void ShootKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (magazine != 0)
            {
                if (isReloadTimeActive == false)
                {
                    //待機アニメーション
                    isAnimIdle = false;
                    animator.SetBool("b_Idle", isAnimIdle);
                    idleTime = 0.0f;

                    magazine = magazine - 1;//残弾数を-1する

                    //SEオブジェクトを生成する
                    UnityEngine.GameObject se = Instantiate(bulletSEPrefab, this.transform.position, Quaternion.identity);
                    Destroy(se, bulletSEDestroyTime);//SEをSE_Endtime後削除

                    if (isRotRight == true)
                    {

                        //マズルフラッシュエフェクトオブジェクトを生成する	
                        Vector3 rightMuzzleflashEffectPosition = new Vector3(this.transform.position.x + 1.75f, this.transform.position.y + 1.1f, this.transform.position.z);
                        UnityEngine.GameObject newEffect = Instantiate(rightMuzzleflashEffectPrefab, rightMuzzleflashEffectPosition, Quaternion.identity);
                        Destroy(newEffect, muzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                        //弾を生成する
                        UnityEngine.Vector3 rightBulletPosition = new Vector3(this.transform.position.x + 1.75f, this.transform.position.y + 1.1f, this.transform.position.z);
                        UnityEngine.GameObject newBullet = Instantiate(bullet, rightBulletPosition, this.transform.rotation);
                        newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                        //薬莢を生成する
                        UnityEngine.Vector3 rightCartridgePosition = new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.8f, this.transform.position.z);
                        UnityEngine.GameObject newCartridge = Instantiate(cartridge, rightCartridgePosition, this.transform.rotation);
                        newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);
                        newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);
                        Destroy(newCartridge, cartridgeDestroyTime);//DestroyTime後削除
                    }
                    else if (isRotRight == false)
                    {

                        //マズルフラッシュエフェクトオブジェクトを生成する	
                        Vector3 leftMuzzleflashEffectPosition = new Vector3(this.transform.position.x - 1.6f, this.transform.position.y + 1.0f, this.transform.position.z);
                        UnityEngine.GameObject newEffect = Instantiate(leftMuzzleflashEffectPrefab, leftMuzzleflashEffectPosition, Quaternion.identity);
                        Destroy(newEffect, muzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

                        //弾を生成する
                        UnityEngine.Vector3 leftBulletPosition = new Vector3(this.transform.position.x - 1.8f, this.transform.position.y + 0.9f, this.transform.position.z);
                        UnityEngine.GameObject newBullet = Instantiate(bullet, leftBulletPosition, this.transform.rotation);
                        newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 2500.0f);//速すぎるとすり抜けてしまう

                        //薬莢を生成する
                        UnityEngine.Vector3 leftCartridgePosition = new Vector3(this.transform.position.x + 0.4f, this.transform.position.y + 0.8f, this.transform.position.z);
                        UnityEngine.GameObject newCartridge = Instantiate(cartridge, leftCartridgePosition, this.transform.rotation);
                        newCartridge.GetComponent<Rigidbody>().AddForce(transform.up * 100.0f);
                        newCartridge.GetComponent<Rigidbody>().AddForce(transform.right * 300.0f);
                        Destroy(newCartridge, cartridgeDestroyTime);//DestroyTime後削除
                    }
                }
            }
        }
    }

    //android用Reload関数
    public void ReloadTouchButton()
    {
        if (magazine != 20)
        {
            isReloadTimeActive = true;//リロードのオン
        }
    }

    void ReloadKeyboard()
    {
        if (Input.GetKey(KeyCode.R) && magazine != 20)
        {
            isReloadTimeActive = true;//リロードのオン
        }
    }

    void ReloadUpdateSystem()
    {
        if (magazine == 0)
        {
            isReloadTimeActive = true;//リロードのオン
        }

        if (isReloadTimeActive == true)//リロードがオンになったら
        {
            if (reloadTime == 0)
            {
                //待機アニメーション
                isAnimIdle = false;
                animator.SetBool("b_Idle", isAnimIdle);
                idleTime = 0.0f;

                //SEオブジェクトを生成する
                UnityEngine.GameObject reloadSE = Instantiate(reloadSEPrefab, this.transform.position, Quaternion.identity);
                Destroy(reloadSE, reloadSEDestroyTime);//SEをSE_Endtime後削除

                //アニメーション
                isAnimReload = true;
                animator.SetBool("b_Reload", isAnimReload);
            }
            //リロード中画像
            reloadTime += Time.deltaTime;//リロードタイムをプラス

            if (reloadTimeDefine <= reloadTime)//リロードタイムが10以上になったら
            {
                magazine = magazineDefine;//弾リセット
                reloadTime = 0.0f;//リロードタイムをリセット
                isReloadTimeActive = false;//リロードのオフ
                isAnimReload = false;//リロードアニメーションのオフ
                animator.SetBool("b_Reload", isAnimReload);
            }
        }
    }

    void HPSystem()
    {
        //体力
        if (hp <= 0)
        {
            //アニメーション
            isAnimDie = true;
            animator.SetBool("b_Die", isAnimDie);

            isGameOverTrigger = true;
#if UNITY_ANDROID//端末がAndroidだった場合の処理
            adsInterstitial.ShowAd();//広告表示
#endif
            StageSceneController.GameOver(gameOverDelay);
        }

        //プレイヤーダメージSE
        playerDamageTime += Time.deltaTime;
        //Debug.Log("プレイヤーダメージSE" + PlayerDamageTime);

        //ダメージ
        if (isDamage == true)
        {
            UI.singletonInstance.imageDamage.color = new Color(0.5f, 0f, 0f, 0.5f);

            //血エフェクトオブジェクトを生成する	
            Vector3 bloodEffectPosition = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z);
            UnityEngine.GameObject newEffect = Instantiate(bloodEffectPrefab, bloodEffectPosition, Quaternion.identity);
            Destroy(newEffect, bloodEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
        }

        if (isDamage == false)
        {
            UI.singletonInstance.imageDamage.color = Color.Lerp(UI.singletonInstance.imageDamage.color, Color.clear, Time.deltaTime);
        }

        isDamage = false;

        //救急箱エフェクト
        if (isHeal == true)
        {
            UI.singletonInstance.imageHeal.color = new Color(0f, 0.5f, 0f, 0.5f);

            healTime = 0.0f;
        }

        if (isHeal == false)
        {
            UI.singletonInstance.imageHeal.color = Color.Lerp(UI.singletonInstance.imageHeal.color, Color.clear, Time.deltaTime);
        }

        isHeal = false;

        if (isHealEffect == true)
        {
            //回復エフェクトを生成
            Vector3 healEffectPosition = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z - 1.0f);
            UnityEngine.GameObject healEffect = Instantiate(healEffectPrefab, healEffectPosition, Quaternion.identity);
            Destroy(healEffect, healEffectDestroyTime);
        }

        if (healTime <= healTimeDefine)
        {
            isHealEffect = true;
        }

        if (healTimeDefine <= healTime)
        {
            isHealEffect = false;
        }

        healTime += Time.deltaTime;
    }

    void Hide()
    {
        if (hidables != null)
        {
            foreach (var hidable in hidables)
            {
                if ((Input.GetKeyDown(hidable.HideKey()) || Input.GetKeyDown(KeyCode.W)) && hidable.IsAccessable(gameObject))
                {
                    hidable.Hide(gameObject);
                    break;
                }
            }
        }
    }

    public void SetPlayerDamage(int damage)
    {
        hp = hp - damage;
        if (hp <= 0)
        {
            hp = 0;
        }
    }

    public void SetPlayerHeal(int heal)
    {
        hp = hp + heal;
        if (100 <= hp)
        {
            hp = 100;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Mine"))
        {
            if (playerDamageTimeDefine <= playerDamageTime)
            {
                var se = Instantiate(playerDamageSEPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(se, playerDamageSEDestroyTime);

                playerDamageTime = 0.0f;
            }

            isDamage = true;

            cameraController.Shake(0.25f, 0.1f);

#if UNITY_ANDROID
            //android端末を振動させる(0.5秒程度の振動が1回だけ行われる)
            if (SystemInfo.supportsVibration)
            {
                Handheld.Vibrate();
            }
#endif
        }

        if (other.CompareTag("First aid kit") && hp < 100)
        {
            UnityEngine.GameObject se = Instantiate(healSEPrefab, this.transform.position, Quaternion.identity);
            Destroy(se, healSEDestroyTime);

            isHeal = true;
            SetPlayerHeal(100);
        }
    }
}
