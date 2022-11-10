using Model;
using UnityEngine;
using UnityEngine.UI;

public class Player3D : MonoBehaviour
{
    //アニメーション
    public Animator anim;//Unityアニメーション用変数
    bool isAnimJump = false;
    bool isAnimReload = false;
    bool isAnimDie = false;
    bool isAnimationMove = false;
    bool isAnimIdle = false;
    readonly float idleTimeDefine = 2.0f;//待機アニメーションへ行こうする時間
    float idleTime;

    //移動
    public float speed = 4.0f;
    float moveForce = 50.0f;
    float speedX => rigid.velocity.x;

    //ジャンプ
    Rigidbody rigid;
    float jumpForce = 570.0f;//ジャンプのy軸に加える力
    readonly float jumpLoadTimeDefine = 1.2f;
    float jumpLoadTime;//ジャンプの再使用までのロードタイム
    /// <summary>
    /// 重力補正
    /// </summary>
    public float gravityScale = 1.0f;

    //弾
    public GameObject Bullet;

    //弾発射のSE
    public GameObject BulletSEPrefab;
    float bulletSEDestroyTime = 1.0f;

    //弾数
    public int magazine;//残弾数
    readonly int magazineDefine = 20;
    public bool isReloadTimeActive = false;//リロードのオン・オフ
    float reloadTime = 0.0f;
    readonly float reloadTimeDefine = 1.5f;//リロード時間の固定

    //薬莢
    public GameObject Cartridge;
    float CartridgeDestroyTime = 1.0f;

    //リロードのSE
    public GameObject ReloadSEPrefab;
    float ReloadSEDestroyTime = 1.0f;

    //右マズルフラッシュエフェクトのプレファブ
    public GameObject RightMuzzleflashEffectPrefab;
    private Vector3 RightMuzzleflashEffectPosition;
    float MuzzleflashEffectDestroyTime = 0.5f;

    //左マズルフラッシュエフェクトのプレファブ
    public GameObject LeftMuzzleflashEffectPrefab;
    private Vector3 LeftMuzzleflashEffectPosition;

    //体力
    int hp = 100;

    //ゲームオーバー
    public bool isGameOverTrigger = false;
    public readonly float GameOverDelay = 2.0f;

    //プレイヤーダメージSE
    public GameObject PlayerDamageSEPrefab;
    float playerDamageSEDestroyTime = 1.0f;
    float PlayerDamageTime;
    float PlayerDamageTimeDefine = 1.0f;

    //ダメージ画像
    public bool isImageDamage = false;
    [SerializeField] Image imageDamage;

    //血エフェクト
    public GameObject BloodEffectPrefab;
    Vector3 BloodEffectPosition;
    float bloodEffectDestroyTime = 1.0f;

    //ヒール画像
    bool isImageHeal;
    [SerializeField] Image imageHeal;

    //ヒールSE
    public GameObject HealSEPrefab;
    float healSEDestroyTime = 1.0f;

    //ヒールエフェクトのプレファブ
    public GameObject HealEffectPrefab;
    Vector3 HealEffectPosition;
    float HealEffectDestroyTime = 1.0f;
    bool isHealEffect = false;
    readonly float healTimeDefine = 0.2f;
    float HealTime;

    //回転
    bool rot = true;

    IHidable[] hidables;

    //カメラ
    CameraController cameraController;

    void Start()
    {
        //ジャンプ
        rigid = this.GetComponent<Rigidbody>();
        jumpLoadTime = jumpLoadTimeDefine;//ジャンプの再使用までのロードタイム

        //ダメージ画像
        imageDamage.color = Color.clear;

        //ヒール画像
        imageHeal.color = Color.clear;
        //ヒールエフェクト
        HealTime = healTimeDefine;

        //アニメーション
        anim = this.GetComponent<Animator>();//アニメーションのコンポーネントを探す

        //弾数
        magazine = magazineDefine;//残弾数

        hidables = GameObject.Find("Hidable").GetComponentsInChildren<IHidable>();

        //カメラ
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
    }

    void Update()
    {
        if (isGameOverTrigger) { return; }

        Jump();

        Move();

        Shoot();

        Reload();

        HP();

        Hide();
    }

    void Move()
    {
        //現在のアニメーション（"Speed"）の値を持ってくる
        float animationCurrentPlayerMoveSpeed = anim.GetFloat("f_CurrentPlayerMoveSpeed");

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

        //移動
        if (Input.GetKey("d") && isGameOverTrigger == false)
        {
            //待機アニメーション
            isAnimIdle = false;
            anim.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

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

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            anim.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //移動
            if (speedX < speed)
            {
                rigid.AddForce(moveForce * Vector3.right);
            }
            //transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey("a") && isGameOverTrigger == false)
        {
            //待機アニメーション
            isAnimIdle = false;
            anim.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

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

            isAnimationMove = true;
            //移動アニメーションを徐々に「歩き」状態にする
            anim.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed + Time.deltaTime * 1.0f);

            //移動
            if (speedX > -speed)
            {
                rigid.AddForce(moveForce * Vector3.left);
            }
            //transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (isAnimationMove == false)
        {
            //移動アニメーションを徐々に「立ち」状態にする
            anim.SetFloat("f_CurrentPlayerMoveSpeed", animationCurrentPlayerMoveSpeed - Time.deltaTime * 1.0f);
        }

        isAnimationMove = false;//アニメーションの移動をFalseにする

        //待機アニメーション
        if (idleTimeDefine <= idleTime)
        {
            isAnimIdle = true;
            anim.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;
        }

        if (isAnimIdle == false)
        {
            idleTime += Time.deltaTime;
        }
    }

    void Jump()
    {
        rigid.velocity += (gravityScale - 1) * Physics.gravity * Time.deltaTime;

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && jumpLoadTimeDefine <= jumpLoadTime && isGameOverTrigger == false)
        {
            //アニメーション
            isAnimJump = true;
            anim.SetBool("b_Jump", isAnimJump);

            rigid.AddForce(transform.up * jumpForce);
            jumpLoadTime = 0.0f;
        }

        if (jumpLoadTime <= jumpLoadTimeDefine)
        {
            jumpLoadTime += Time.deltaTime;
        }

        if (jumpLoadTime >= 0.1f)
        {
            isAnimJump = false;
        }
        anim.SetBool("b_Jump", isAnimJump);
    }

    void Shoot()
    {
        //弾
        if ((Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(0)) && (magazine != 0) && isReloadTimeActive == false && isGameOverTrigger == false)
        {
            //待機アニメーション
            isAnimIdle = false;
            anim.SetBool("b_Idle", isAnimIdle);
            idleTime = 0.0f;

            magazine = magazine - 1;//残弾数を-1する

            //SEオブジェクトを生成する
            var se = Instantiate(BulletSEPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(se, bulletSEDestroyTime);//SEをSE_Endtime後削除

            var PositionX = gameObject.transform.position.x;
            var PositionY = gameObject.transform.position.y;
            var PositionZ = gameObject.transform.position.z;

            if (rot)
            {

                //マズルフラッシュエフェクトオブジェクトを生成する	
                RightMuzzleflashEffectPosition = new Vector3(PositionX + 1.75f, PositionY + 1.1f, PositionZ);
                var effect = Instantiate(RightMuzzleflashEffectPrefab, RightMuzzleflashEffectPosition, Quaternion.identity);
                Destroy(effect, MuzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

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
                var effect = Instantiate(LeftMuzzleflashEffectPrefab, LeftMuzzleflashEffectPosition, Quaternion.identity);
                Destroy(effect, MuzzleflashEffectDestroyTime);//エフェクトをEffectDestroyTime後削除

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
    }

    void Reload()
    {
        //リロードシステム
        if (magazine == 0 || (magazine != 20 && Input.GetKey(KeyCode.R)) && isGameOverTrigger == false)
        {
            isReloadTimeActive = true;//リロードのオン
        }

        if (isReloadTimeActive)//リロードがオンになったら
        {
            if (reloadTime == 0)
            {
                //待機アニメーション
                isAnimIdle = false;
                anim.SetBool("b_Idle", isAnimIdle);
                idleTime = 0.0f;

                //SEオブジェクトを生成する
                var ReloadSE = Instantiate(ReloadSEPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(ReloadSE, ReloadSEDestroyTime);//SEをSE_Endtime後削除

                //アニメーション
                isAnimReload = true;
                anim.SetBool("b_Reload", isAnimReload);
            }
            //リロード中画像
            reloadTime += Time.deltaTime;//リロードタイムをプラス

            if (reloadTimeDefine <= reloadTime)//リロードタイムが10以上になったら
            {
                magazine = magazineDefine;//弾リセット
                reloadTime = 0.0f;//リロードタイムをリセット
                isReloadTimeActive = false;//リロードのオフ
                isAnimReload = false;//リロードアニメーションのオフ
                anim.SetBool("b_Reload", isAnimReload);
            }
        }
        //リロードシステム
    }

    void HP()
    {
        //体力
        if (!isGameOverTrigger && hp <= 0)
        {
            //アニメーション
            isAnimDie = true;
            anim.SetBool("b_Die", isAnimDie);
            isGameOverTrigger = true;
            StageSceneController.GameOver(GameOverDelay);
        }

        //プレイヤーダメージSE
        PlayerDamageTime += Time.deltaTime;
        //Debug.Log("プレイヤーダメージSE" + PlayerDamageTime);

        //ダメージ画像
        if (isImageDamage)
        {
            imageDamage.color = new Color(0.5f, 0f, 0f, 0.5f);
            //血エフェクトオブジェクトを生成する	
            //血エフェクト座標
            BloodEffectPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z);
            var effect = Instantiate(BloodEffectPrefab, BloodEffectPosition, Quaternion.identity);
            Destroy(effect, bloodEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
        }

        if (isImageDamage == false)
        {
            imageDamage.color = Color.Lerp(imageDamage.color, Color.clear, Time.deltaTime);
        }

        isImageDamage = false;

        //救急箱エフェクト
        if (isImageHeal)
        {
            imageHeal.color = new Color(0f, 0.5f, 0f, 0.5f);

            HealTime = 0.0f;
        }

        if (isImageHeal == false)
        {
            imageHeal.color = Color.Lerp(imageHeal.color, Color.clear, Time.deltaTime);
        }

        isImageHeal = false;

        if (isHealEffect)
        {
            HealEffectPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.0f, gameObject.transform.position.z - 1.0f);

            var healEffect = Instantiate(HealEffectPrefab, HealEffectPosition, Quaternion.identity);
            Destroy(healEffect, HealEffectDestroyTime);//エフェクトをEffectDestroyTime後削除
        }

        if (HealTime <= healTimeDefine)
        {
            isHealEffect = true;
        }


        if (healTimeDefine <= HealTime)
        {
            isHealEffect = false;
        }

        HealTime += Time.deltaTime;
    }

    void Hide()
    {
        foreach (var hidable in hidables)
        {
            if (Input.GetKeyDown(hidable.HideKey()) && hidable.IsAccessable(gameObject))
            {
                hidable.Hide(gameObject);
                break;
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

    public int GetPlayerHP()
    {
        return hp;
    }

    public float GetPlayerPositionX()
    {
        return this.transform.position.x;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") && isGameOverTrigger == false || other.CompareTag("Mine") && isGameOverTrigger == false)
        {
            if (PlayerDamageTimeDefine <= PlayerDamageTime)
            {
                var se = Instantiate(PlayerDamageSEPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(se, playerDamageSEDestroyTime);

                PlayerDamageTime = 0.0f;
            }

            isImageDamage = true;

            //cameraController.Shake(0.25f, 0.1f);
        }

        if (other.CompareTag("First aid kit") && hp < 100 && isGameOverTrigger == false)
        {
            var se = Instantiate(HealSEPrefab, gameObject.transform.position, Quaternion.identity);
            Destroy(se, healSEDestroyTime);

            isImageHeal = true;
            SetPlayerHeal(100);
        }
    }
}
